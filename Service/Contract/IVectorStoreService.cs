using DevAssistAI.Model;

namespace DevAssistAI.Service.Contract
{
    public interface IVectorStoreService
    {
        Task<List<DocumentVector>> GetDocumentVectors();
    }
}
