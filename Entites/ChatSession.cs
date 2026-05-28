using System.Data;

namespace DevAssistAI.Entites
{
    public class ChatSession
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
