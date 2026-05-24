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
        private readonly IVectorStoreService _vectorStoreService;

        public AIService(IHttpClientFactory httpClientFactory, IConfiguration configuration, IChatMemoryService chatMemoryService, IVectorStoreService vectorStoreService)
        {
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
            _chatMemoryService = chatMemoryService;
            _vectorStoreService = vectorStoreService;
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
                model = _configuration["Ollama:ChatModel"],
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
            List<ChatMessage> message = _chatMemoryService.GetChatHistory();

            if (!message.Any())
            {
                message.Add(new ChatMessage
                {
                    Role = "system",
                    Content = $@"
                    You are an expert .NET and Generative AI mentor.
                   
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
                model = _configuration["Ollama:ChatModel"],
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

        public async Task<string> RagChat(AIRequest request)
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
                model = _configuration["Ollama:ChatModel"],
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

        public async Task<string> SemanticRAG(AIRequest request)
        { 
            // GENERATE PROMPT EMBEDDING
            List<float> promptEmbedding = await GenerateEmbedding(request.Prompt ?? string.Empty);

            //GET DOCUMENT VECTORS
            List<DocumentVector> documentVectors = await _vectorStoreService.GetDocumentVectors();


            // STORE DOCUMENT SCORES
            List<(DocumentVector Doc, float Score)> scoredDocuments = new List<(DocumentVector Doc, float Score)>();

            // GENERATE DOC EMBEDDINGS + CALCULATE SIMILARITY
            foreach(DocumentVector doc in documentVectors)
            {
                float similarity = CalculateCosineSimilarity(promptEmbedding, doc.Embedding);

                scoredDocuments.Add((doc, similarity));
            }

            //GET TOP 3 MOST RELEVANT DOCUMENT
            List<(DocumentVector Doc, float Score)> bestDocument = scoredDocuments.OrderByDescending(d => d.Score).Take(3).ToList();

            StringBuilder contextBuilder = new StringBuilder();

            //PREPARE CONTEXT
            foreach ((DocumentVector Doc, float Score) doc in bestDocument)
            {
                contextBuilder.AppendLine(doc.Doc.Content);
                contextBuilder.AppendLine("\n--------------------------\n");
            }
            string relevantContext = contextBuilder.ToString();

            List<ChatMessage> chatHistory = _chatMemoryService.GetChatHistory();

            List<ChatMessage> message = new List<ChatMessage>();

            
            message.Add(new ChatMessage
            {
                Role = "system",
                Content = $@"
                You are an expert .NET and Generative AI mentor.

                Use the following knowledge while answering:
                
                {relevantContext}
               
                Rules:
                - Explain in simple beginner-friendly language
                - Give practical examples
                - Keep answers clear and structured
                - Avoid overly complex terminology
                "
            });

            message.AddRange(chatHistory);

            message.Add(new ChatMessage
            {
                Role = "user",
                Content = request.Prompt ?? string.Empty
            });

            var ollamaRequest = new
            {
                model = _configuration["Ollama:ChatModel"],
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

        private float CalculateCosineSimilarity(  List<float> vector1,  List<float> vector2)
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
