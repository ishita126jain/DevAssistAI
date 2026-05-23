using DevAssistAI.Model;

namespace DevAssistAI.Service.Contract
{
    public interface IAIService
    {
        Task<string> AskAI(AIRequest request);
        Task<string> ChatAI(AIRequest request);
    }
}
