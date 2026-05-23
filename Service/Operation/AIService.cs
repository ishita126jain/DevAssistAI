using DevAssistAI.Model;
using DevAssistAI.Service.Contract;
using System.Text;
using System.Text.Json;

namespace DevAssistAI.Service.Operation
{
    public class AIService : IAIService
    {

        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IChatMemoryService _chatMemoryService;

        public AIService(IHttpClientFactory httpClientFactory, IConfiguration configuration, IChatMemoryService chatMemoryService)
        {
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
            _chatMemoryService = chatMemoryService;
        }
        public async Task<string> AskAI(AIRequest request)
        {
            string? systemPrompt = @"
            You are an expert .NET and Generative AI mentor.
            
            Rules:
            - Explain in simple beginner-friendly language
            - Give practical examples
            - Keep answers clear and structured
            - Avoid overly complex terminology
            ";

            var ollamaRequest = new
            {
                model = _configuration["Ollama:Model"],
                prompt = $"{systemPrompt}\n\nUser Question: {request.Prompt}",
                stream = false
            };

            StringContent content = new StringContent(JsonSerializer.Serialize(ollamaRequest), Encoding.UTF8, "application/json");
            HttpResponseMessage? response = await _httpClient.PostAsync(_configuration["Ollama:GenerateEndpoint"], content);
            string? result = await response.Content.ReadAsStringAsync();
            OllamaResponse? ollamaResponse = JsonSerializer.Deserialize<OllamaResponse>(result);
            return ollamaResponse?.response ?? "No response from AI";
        }

        public async Task<string> ChatAI(AIRequest request)
        {
            string knowledgeFolder = Path.Combine(Directory.GetCurrentDirectory(), "KnowledgeBase");
            string[]? files = Directory.GetFiles(knowledgeFolder, "*.txt");

            List<KnowledgeDocument> documents = new List<KnowledgeDocument>();

            foreach (string file in files)
            {
                string fileContent = await File.ReadAllTextAsync(file);

                documents.Add(new KnowledgeDocument
                {
                    FileName = Path.GetFileName(file),
                    Content = fileContent
                });

            }

            List<KnowledgeDocument> relevantDocument = documents.Where(d => (request.Prompt ?? string.Empty).ToLower().Contains(Path.GetFileNameWithoutExtension(d.FileName).ToLower())).ToList();

            StringBuilder? combinedKnowledge = new StringBuilder();

            if (!relevantDocument.Any())
            {
                combinedKnowledge.AppendLine("No specific knowledge document found.");
            }

            foreach (KnowledgeDocument document in relevantDocument)
            {
                combinedKnowledge.AppendLine(document.Content);
                combinedKnowledge.AppendLine("\n--------------------------\n");
            }

            List<ChatMessage> message = _chatMemoryService.GetChatHistory();

            if (!message.Any())
            {
                message.Add(new ChatMessage
                {
                    Role = "system",
                    Content = $@"
                    You are an expert .NET and Generative AI mentor.
                    
                    Use the following knowledge while answering:
                    
                    {combinedKnowledge}

                    Rules:
                    - Explain in simple beginner-friendly language
                    - Give practical examples
                    - Keep answers clear and structured
                    - Avoid overly complex terminology
                    "
                });
            }

            message.Add(new ChatMessage
            {
                Role = "user",
                Content = request.Prompt ?? string.Empty
            });
            
            var ollamaRequest = new
            {
                model = _configuration["Ollama:Model"],
                messages = message,
                stream = false
            };

            StringContent content = new StringContent(JsonSerializer.Serialize(ollamaRequest), Encoding.UTF8, "application/json");
            HttpResponseMessage? response = await _httpClient.PostAsync(_configuration["Ollama:ChatEndpoint"], content);
            string? result = await response.Content.ReadAsStringAsync();
            ChatResponse? chatResponse = JsonSerializer.Deserialize<ChatResponse>(result);

            _chatMemoryService.AddMessage(new ChatMessage
            {
                Role = "assistant",
                Content = chatResponse?.message?.content ?? "No response from AI"
            });

            return chatResponse?.message?.content ?? "No response from AI";
        }
    }
}
