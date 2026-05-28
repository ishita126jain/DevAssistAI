namespace DevAssistAI.Entites
{
    public class ChatMessages
    {
        public Guid Id { get; set; }
        public Guid ChatSessionId {  get; set; }
        public string Role {  get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt {  get; set; }
        public ChatSession? ChatSession { get; set; }
    }
}
