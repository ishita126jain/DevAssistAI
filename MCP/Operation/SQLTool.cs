using DevAssistAI.MCP.Contact;

namespace DevAssistAI.MCP.Operation
{
    public class SQLTool : IMCPTool
    {
        public string Name => "SQLTool";
        public bool CanHandle(string prompt)
        {
            return prompt.Contains("sql", StringComparison.OrdinalIgnoreCase)
                || prompt.Contains("query", StringComparison.OrdinalIgnoreCase)
                || prompt.Contains("database", StringComparison.OrdinalIgnoreCase);
        }
        public Task<string> Execute(string prompt)
        {
            return Task.FromResult(@"
            SQL is used to communicate with databases.
            
            Common SQL commands:
            - SELECT
            - INSERT
            - UPDATE
            - DELETE
            
            SQL databases are commonly used in ASP.NET Core applications.");
        }
    }
}
