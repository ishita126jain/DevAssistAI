using DevAssistAI.Model;
using DevAssistAI.Service.Contract;
using Microsoft.AspNetCore.Identity;
using System.Text;
using System.Text.Json;

namespace DevAssistAI.Service.Operation
{
    public class VectorStoreService : IVectorStoreService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        private List<DocumentVector> _documentVectors = new();

        public VectorStoreService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
        }
        public async Task<List<DocumentVector>> GetDocumentVectors()
        {
            string knowledgeFolder = Path.Combine(Directory.GetCurrentDirectory(), "KnowledgeBase");
            string[]? files = Directory.GetFiles(knowledgeFolder, "*.txt");

            foreach(string file in files)
            {
                string fileName = Path.GetFileName(file);

                bool alreadyExisits = _documentVectors.Any(d => d.FileName.Equals(fileName, StringComparison.OrdinalIgnoreCase));

                if (alreadyExisits) continue;

                //IF NEW FILE, THEN READ CONTENT AND GENERATE VECTOR
                string content = await File.ReadAllTextAsync(file);

                List<float> embedding = await GenerateEmbedding(content);

                _documentVectors.Add(new DocumentVector
                {
                    FileName = fileName,
                    Content = content,
                    Embedding = embedding
                });
            }

            return _documentVectors;
        }
        private async Task<List<float>> GenerateEmbedding(string prompt)
        {
            EmbeddingRequest embeddingRequest = new EmbeddingRequest
            {
                Model = _configuration["Ollama:EmbeddingModel"] ?? string.Empty,
                Prompt = prompt ?? string.Empty
            };

            StringContent content = new StringContent(JsonSerializer.Serialize(embeddingRequest), Encoding.UTF8, "application/json");
            HttpResponseMessage? response = await _httpClient.PostAsync(_configuration["Ollama:EmbeddingEndpoint"], content);
            string? result = await response.Content.ReadAsStringAsync();
            EmbeddingResponse? embeddingResponse = JsonSerializer.Deserialize<EmbeddingResponse>(result);

            return embeddingResponse?.embedding ?? new List<float>();
        }
    }
}
