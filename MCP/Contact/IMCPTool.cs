namespace DevAssistAI.MCP.Contact
{
    public interface IMCPTool
    {
        string Name { get; }
        bool CanHandle(string prompt);
        Task<string> Execute(string prompt);
    }
}
