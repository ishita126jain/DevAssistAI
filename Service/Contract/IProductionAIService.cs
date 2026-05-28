using DevAssistAI.DTO;
using DevAssistAI.Entites;

namespace DevAssistAI.Service.Contract
{
    public interface IProductionAIService
    {
        Task<ProductionChatResponse> Chat(ProductionChatRequest request, CancellationToken cancellationToken);
        Task<List<ChatSessionResponse>> GetSessions();
        Task<List<ChatMessageResponse>> GetMessages(Guid sessionId);
    }
}
