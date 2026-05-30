namespace DevAssistAI.Model
{
    public class DocumentChunk
    {
        public Guid Id { get; set; }

        public string FileName { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public string EmbeddingJson { get; set; } = string.Empty;

        public int ChunkIndex { get; set; }

        public string FileHash { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }
    }
}
