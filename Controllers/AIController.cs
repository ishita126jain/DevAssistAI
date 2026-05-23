using DevAssistAI.Model;
using DevAssistAI.Service.Contract;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace DevAssistAI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIController : ControllerBase
    {
        private readonly IAIService _aiService;

        public AIController(IAIService aiService)
        {
            _aiService = aiService;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> AskAI([FromBody] AIRequest request)
        {
            string response = await _aiService.AskAI(request);

            return Ok(response);
        }

        [HttpPost("chatAI")]
        public async Task<IActionResult> ChatAI([FromBody] AIRequest request)
        {
            string response = await _aiService.ChatAI(request);
            return Ok(response);
        }
    }
}
