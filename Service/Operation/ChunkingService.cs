using DevAssistAI.Data;
using DevAssistAI.Model;
using DevAssistAI.Repository.Contract;
using DevAssistAI.Service.Contract;
using Microsoft.AspNetCore.SignalR;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DevAssistAI.Service.Operation
{
    public class ChunkingService : IChunkingService
    {
        private readonly ILogger<ChunkingService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IDocumentChunkRepository _documentChunkRepository;
        private readonly IEmbeddingService _embeddingService;

        public ChunkingService(ILogger<ChunkingService> logger, IConfiguration configuration, IDocumentChunkRepository documentChunkRepository, IEmbeddingService embeddingService)
        {
            _logger = logger;
            _configuration = configuration;
            _documentChunkRepository = documentChunkRepository;
            _embeddingService = embeddingService;
        }

        public async Task ProcessFolder(string folderPath, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Started processing folder: {FolderPath}", folderPath);

            //Check folder exists
            if (!Directory.Exists(folderPath))
            {
                _logger.LogWarning("Folder not found: {FolderPath}", folderPath);
                throw new DirectoryNotFoundException($"Folder not found: {folderPath}");
            }

            //Get all txt files
            string[] files = Directory.GetFiles(folderPath, "*.txt");

            _logger.LogInformation("Total files found: {Count}", files.Length);


            //Process each file
            foreach (string file in files)
            {
                cancellationToken.ThrowIfCancellationRequested();

                string fileName = Path.GetFileName(file);

                _logger.LogInformation("Processing file: {FileName}", fileName);

                //Read File Content
                string content = await File.ReadAllTextAsync(file, cancellationToken);

                //Generate file hash
                string fileHash = GenerateFileHash(content);

                //Get existing chunks with same file hash
                string existingFileHash = await _documentChunkRepository.GetActiveFileHash(fileName, cancellationToken);

                if (existingFileHash == fileHash)
                {
                    _logger.LogInformation("File already processed: {FileName}", fileName);
                    continue;
                }

                if(!string.IsNullOrEmpty(existingFileHash))
                {
                    await _documentChunkRepository.DeactivateChunks(fileName, cancellationToken);
                }

                //Split into chunks
                List<string> chunks = SplitIntoChunks(content);

                _logger.LogInformation("Total chunks created: {Count}", chunks.Count);

                //Process each chunks
                List<DocumentChunk> documentChunks = new();

                for (int i=0; i< chunks.Count; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    //Generate embedding
                    List<float> embedding = await _embeddingService.GenerateEmbedding(chunks[i], cancellationToken);

                    //Serialize embedding
                    string embeddingJson = JsonSerializer.Serialize(embedding);

                    //Insert into DocumentChunk
                    DocumentChunk documentChunk = new DocumentChunk
                    {
                        Id = Guid.NewGuid(),
                        FileName = fileName,
                        Content = chunks[i],
                        EmbeddingJson = embeddingJson,
                        ChunkIndex = i,
                        FileHash = fileHash,
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    };
                    documentChunks.Add(documentChunk);
                }

                //Save to db
                await _documentChunkRepository.AddDocumentChunk(documentChunks, cancellationToken);
                _logger.LogInformation( "Created {ChunkCount} chunks for file {FileName}", chunks.Count, fileName);

                _logger.LogInformation("Completed processing file: {FileName}", fileName);
            }
            _logger.LogInformation("Folder processing completed: {FolderPath}", folderPath);
        }

        private List<string> SplitIntoChunks(string content)
        {
            return content.Split( new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        }

        private string GenerateFileHash(string content)
        {
            using SHA256 sha256 = SHA256.Create();
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(content));

            return Convert.ToHexString(hashBytes);
        }
    }
}


