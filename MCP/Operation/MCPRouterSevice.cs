using DevAssistAI.MCP.Contact;

namespace DevAssistAI.MCP.Operation
{
    public class MCPRouterSevice : IMCPRouterService
    {
        private readonly IEnumerable<IMCPTool> _tools;
        public MCPRouterSevice(IEnumerable<IMCPTool> tools)
        {
            _tools = tools;
        }

        public async Task<string?> Route (string prompt)
        {
            foreach(IMCPTool tool in _tools)
            {
                if (tool.CanHandle(prompt))
                {
                    return await tool.Execute(prompt);
                }
            }
            return null;
        }
    }
}
