using DevAssistAI.Model;
using DevAssistAI.Service.Contract;

namespace DevAssistAI.Service.Operation
{
    public class ChatMemoryService : IChatMemoryService
    {
        private readonly List<ChatMessage> _chatHistory = new();

        public List<ChatMessage> GetChatHistory()
        {
            return _chatHistory;
        }

        public void AddMessage(ChatMessage message)
        {
            if (message.Role == "system")
            {
                return;
            }
            _chatHistory.Add(message);
        }
        public void ClearChatHistory()
        {
            _chatHistory.Clear();
        }
    }
}
