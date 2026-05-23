using DevAssistAI.Model;

namespace DevAssistAI.Service.Contract
{
    public interface IChatMemoryService
    {
        List<ChatMessage> GetChatHistory();
        void AddMessage(ChatMessage message);
        void ClearChatHistory();
    }
}
