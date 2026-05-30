namespace DevAssistAI.DTO
{
    public class ProductionChatResponse
    {
        public Guid SessionId { get; set; }

        public string Response { get; set; } = string.Empty;
        public List<string> Sources { get; set; } = new();
    }
}
