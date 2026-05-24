using DevAssistAI.Model;

namespace DevAssistAI.Service.Contract
{
    public interface IAIService
    {
        Task<string> AskAI(AIRequest request);
        Task<string> ChatAI(AIRequest request);
        Task<string> RagChat(AIRequest request);
        Task<string> SemanticRAG(AIRequest request);
    }
}
