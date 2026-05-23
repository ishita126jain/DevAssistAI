namespace DevAssistAI.Model
{
    public class ChatResponse
    {
        public Message message { get; set; } = new Message();  
    }
    public class Message
    {
        public string role { get; set; } = string.Empty;
        public string content { get; set; } = string.Empty;
    }
}
