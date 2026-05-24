namespace DevAssistAI.Model
{
    public class DocumentVector
    {
        public string FileName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public List<float> Embedding { get; set; } = new();
    }
}
