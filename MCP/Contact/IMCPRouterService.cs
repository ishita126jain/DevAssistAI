namespace DevAssistAI.MCP.Contact
{
    public interface IMCPRouterService
    {
        Task<string?> Route(string prompt);
    }
}
