using DevAssistAI.Model;

namespace DevAssistAI.Repository.Contract
{
    public interface IDocumentChunkRepository
    {
        Task<string> GetActiveFileHash(string fileName, CancellationToken cancellationToken);
        Task AddDocumentChunk(List<DocumentChunk> chunk, CancellationToken cancellationToken);
        Task DeactivateChunks(string fileName, CancellationToken cancellationToken);
        Task<List<DocumentChunk>> GetActiveChunks(CancellationToken cancellationToken);
    }
}
