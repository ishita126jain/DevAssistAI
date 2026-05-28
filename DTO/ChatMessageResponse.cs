namespace DevAssistAI.DTO
{
    public class ChatMessageResponse
    {
        public string Role { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
