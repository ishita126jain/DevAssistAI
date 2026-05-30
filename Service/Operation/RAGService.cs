using DevAssistAI.DTO;
using DevAssistAI.Model;
using DevAssistAI.Repository.Contract;
using DevAssistAI.Service.Contract;
using System.Text;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace DevAssistAI.Service.Operation
{
    public class RAGService : IRAGService
    {
        private readonly IDocumentChunkRepository _documentChunkRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RAGService> _logger;
        private readonly IEmbeddingService _embeddingService;

        public RAGService( IDocumentChunkRepository documentChunkRepository, IConfiguration configuration, ILogger<RAGService> logger, IEmbeddingService embeddingService)
        {
            _documentChunkRepository = documentChunkRepository;
            _configuration = configuration;
            _logger = logger;
            _embeddingService = embeddingService;
        }

        public async Task<List<RetrievedChunk>> GetRelevantContext(string prompt, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting relevant context for prompt: {Prompt}", prompt);
            
            List<float> queryEmbedding = await _embeddingService.GenerateEmbedding(prompt, cancellationToken);

            //Get active chunks
            List<DocumentChunk> activeChunks = await _documentChunkRepository.GetActiveChunks(cancellationToken);

            //Similarity
            List<(DocumentChunk Chunk, float Score)> scoredChunks = new();

            foreach(DocumentChunk chunk in activeChunks)
            {
                List<float>? embedding = JsonSerializer.Deserialize<List<float>>(chunk.EmbeddingJson);

                if (embedding == null) continue;

                float similarity = CalculateCosineSimilarity( queryEmbedding, embedding);
                scoredChunks.Add((chunk, similarity));
                _logger.LogInformation("Chunk: {FileName} | Score: {Score}", chunk.FileName, similarity);
            }

            //Select Top 5 Chunks
            List<RetrievedChunk> topChunks = scoredChunks.OrderByDescending(x => x.Score).Take(5).Select(x => new RetrievedChunk
            {
                FileName = x.Chunk.FileName,
                Content = x.Chunk.Content,
                Score = x.Score,
            }).ToList();

            return topChunks;
        }

        private float CalculateCosineSimilarity(List<float> vector1, List<float> vector2)
        {
            float dotProduct = 0;
            float magnitude1 = 0;
            float magnitude2 = 0;

            for (int i = 0; i < vector1.Count; i++)
            {
                dotProduct += vector1[i] * vector2[i];

                magnitude1 += vector1[i] * vector1[i];

                magnitude2 += vector2[i] * vector2[i];
            }

            magnitude1 = MathF.Sqrt(magnitude1);

            magnitude2 = MathF.Sqrt(magnitude2);

            return dotProduct / (magnitude1 * magnitude2);
        }

    }
}
