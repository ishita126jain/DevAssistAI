using DevAssistAI.Model;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace DevAssistAI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public AIController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> AskAI([FromBody] AIRequest request)
        {
            var ollamaRequest = new
            {
                model = "gemma3:4b",
                prompt = request.Question,
                stream = false
            };

           StringContent content = new StringContent(JsonSerializer.Serialize(ollamaRequest), Encoding.UTF8, "application/json");
           HttpResponseMessage? response = await _httpClient.PostAsync(_configuration["Ollama:GenerateEndpoint"], content);
           string? result = await response.Content.ReadAsStringAsync();
           OllamaResponse? ollamaResponse = JsonSerializer.Deserialize<OllamaResponse>(result);
           return Ok(ollamaResponse?.response);
        }
    }
}
