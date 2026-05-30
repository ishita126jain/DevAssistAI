namespace DevAssistAI.Service.Contract
{
    public interface IEmbeddingService
    {
        Task<List<float>> GenerateEmbedding(string text, CancellationToken cancellationToken);
    }
}
