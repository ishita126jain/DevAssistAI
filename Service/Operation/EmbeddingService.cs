using DevAssistAI.Model;
using DevAssistAI.Service.Contract;
using System.Text;
using System.Text.Json;

namespace DevAssistAI.Service.Operation
{
    public class EmbeddingService : IEmbeddingService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmbeddingService> _logger;

        public EmbeddingService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<EmbeddingService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        private async Task<List<float>> GenerateEmbedding(string text, CancellationToken cancellationToken)
        {
            HttpClient client = _httpClientFactory.CreateClient("OllamaClient");

            EmbeddingRequest request = new()
            {
                Model = _configuration["Ollama:EmbeddingModel"] ?? string.Empty,
                Prompt = text
            };

            StringContent content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(_configuration["Ollama:EmbeddingEndpoint"], content, cancellationToken);
            response.EnsureSuccessStatusCode();
            string result = await response.Content.ReadAsStringAsync(cancellationToken);
            EmbeddingResponse? embeddingResponse = JsonSerializer.Deserialize<EmbeddingResponse>(result);

            return embeddingResponse?.embedding ?? new List<float>();
        }
    }
}
