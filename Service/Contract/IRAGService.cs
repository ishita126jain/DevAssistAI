using DevAssistAI.DTO;

namespace DevAssistAI.Service.Contract
{
    public interface IRAGService
    {
        Task<List<RetrievedChunk>> GetRelevantContext( string prompt, CancellationToken cancellationToken);
    }
}
