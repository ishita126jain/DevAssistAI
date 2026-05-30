namespace DevAssistAI.Service.Contract
{
    public interface IChunkingService
    {
        Task ProcessFolder(string folderPath, CancellationToken cancellationToken);
    }
}
