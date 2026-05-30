namespace DevAssistAI.DTO
{
    public class RetrievedChunk
    {
        public string FileName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public float Score { get; set; }
    }
}
