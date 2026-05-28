using DevAssistAI.DTO;
using DevAssistAI.Entites;
using DevAssistAI.Exceptions;
using DevAssistAI.Model;
using DevAssistAI.Repository.Contract;
using DevAssistAI.Service.Contract;
using System.Text;
using System.Text.Json;

namespace DevAssistAI.Service.Operation
{
    public class ProductionAIService : IProductionAIService
    {
        private readonly IChatRepository _chatRepository;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ProductionAIService> _logger;

        public ProductionAIService(IChatRepository chatRepository, IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<ProductionAIService> logger)
        {
            _chatRepository = chatRepository;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }
        public async Task<ProductionChatResponse> Chat(ProductionChatRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Chat request received. Prompt: {Prompt}", request.Prompt);

            Guid sessionId;

            if (request.SessionId.HasValue)
            {
                bool sessionExists = await _chatRepository.SessionExists(request.SessionId.Value);
                if (!sessionExists) 
                { 
                    throw new NotFoundException("Session not found");
                }
                sessionId = request.SessionId.Value;
            }
            else
            {
                //Create new Session
                sessionId = Guid.NewGuid();
                _logger.LogInformation( "New session created: {SessionId}", sessionId);

                ChatSession session = new ChatSession
                {
                    Id = sessionId,
                    Title = request.Prompt,
                    CreatedAt = DateTime.Now
                };

                await _chatRepository.CreateSession(session);
            }

            //Add Message
            await _chatRepository.AddMessage(new ChatMessages
            {
                Id = Guid.NewGuid(),
                ChatSessionId = sessionId,
                Role = "user",
                Content = request.Prompt,
                CreatedAt = DateTime.Now
            });

            //Load Chat History
            List<ChatMessages> history = await _chatRepository.GetSessionMessage(sessionId);

            //Add System Prompt
            List<object> messages = new();

            messages.Add(new
            {
                role = "system",
                content = @"
                You are an expert .NET and Generative AI mentor.
                
                Explain in beginner-friendly language.
                
                Keep responses structured and practical.
                "
            });

            // Add DB History
            foreach (ChatMessages chatMessage in history)
            {
                messages.Add(new
                {
                    role = chatMessage.Role,
                    content = chatMessage.Content
                });
            }

            // Call Ollama
            HttpClient client = _httpClientFactory.CreateClient("OllamaClient");

            var ollamaRequest = new
            {
                model = _configuration["Ollama:ChatModel"],
                messages = messages,
                stream = false
            };

            _logger.LogInformation("Sending request to Ollama. Model: {Model}, Messages Count: {MessagesCount}", ollamaRequest.model, messages.Count);

            StringContent content = new StringContent(JsonSerializer.Serialize(ollamaRequest), Encoding.UTF8, "application/json");
            HttpResponseMessage? response = await client.PostAsync(_configuration["Ollama:ChatEndpoint"], content, cancellationToken);
            string? result = await response.Content.ReadAsStringAsync();
            ChatResponse? chatResponse = JsonSerializer.Deserialize<ChatResponse>(result);

            _logger.LogInformation("Received response from Ollama. Response: {Response}", result);

            //Save Assistant Response
            await _chatRepository.AddMessage(new ChatMessages
            {
                Id = Guid.NewGuid(),
                ChatSessionId = sessionId,
                Role = "assistant",
                Content = chatResponse?.message?.content ?? "No response",
                CreatedAt = DateTime.Now
            });

            return new ProductionChatResponse
            {
                SessionId = sessionId,
                Response = chatResponse?.message?.content ?? "No response"
            };
        }

        public async Task<List<ChatSessionResponse>> GetSessions()
        {
            List<ChatSession> sessions = await _chatRepository.GetSessions();

            return sessions.Select(x => new ChatSessionResponse
            {
                Id = x.Id,
                Title = x.Title,
                CreatedAt = x.CreatedAt
            }).ToList();
        }

        public async Task<List<ChatMessageResponse>> GetMessages(Guid sessionId)
        {
            List<ChatMessages> messages =  await _chatRepository.GetMessages(sessionId);

            return messages.Select(x => new ChatMessageResponse
            {
                Role = x.Role,
                Content = x.Content,
                CreatedAt = x.CreatedAt
            }).ToList();
        }
    }
}
