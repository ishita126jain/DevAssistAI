using DevAssistAI.Entites;

namespace DevAssistAI.Repository.Contract
{
    public interface IChatRepository
    {
        Task CreateSession(ChatSession session);
        Task AddMessage(ChatMessages message);
        Task<List<ChatMessages>> GetSessionMessage(Guid sessionId);
        Task<List<ChatSession>> GetSessions();
        Task<List<ChatMessages>> GetMessages(Guid sessionId);
        Task<bool> SessionExists(Guid sessionId);
    }
}
