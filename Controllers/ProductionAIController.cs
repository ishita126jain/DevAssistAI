using DevAssistAI.Common;
using DevAssistAI.DTO;
using DevAssistAI.Entites;
using DevAssistAI.Service.Contract;
using DevAssistAI.Service.Operation;
using Microsoft.AspNetCore.Mvc;

namespace DevAssistAI.Controllers
{
    [ApiController]
    [Route("api/v1/chat")]
    public class ProductionAIController : ControllerBase
    {
        private readonly IProductionAIService _productionAIService;
        private readonly IChunkingService _chunkingService;


        public ProductionAIController(IProductionAIService productionAIService, IChunkingService chunkingService)
        {
            _productionAIService = productionAIService;
            _chunkingService = chunkingService;
        }

        [HttpPost]
        public async Task<IActionResult> Chat(ProductionChatRequest request, CancellationToken cancellationToken)
        {
            ProductionChatResponse response = await _productionAIService.Chat(request, cancellationToken);
            return Ok(new ApiResponse<ProductionChatResponse>
            {
                Success = true,
                Message = "Chat generated successfully",
                Data = response
            });
        }

        [HttpGet("sessions")]
        public async Task<IActionResult> GetSessions()
        {
            List<ChatSessionResponse> response = await _productionAIService.GetSessions();

            return Ok(new ApiResponse<List<ChatSessionResponse>>
            {
                Success = true,
                Message = "Chat sessions retrieved successfully",
                Data = response
            });
        }

        [HttpGet("{sessionId}")]
        public async Task<IActionResult> GetMessages(Guid sessionId)
        {
            List<ChatMessageResponse> reponse = await _productionAIService.GetMessages(sessionId);

            return Ok(new ApiResponse<List<ChatMessageResponse>>
            {
                Success = true,
                Message = "Chat messages retrieved successfully",
                Data = reponse
            });
        }

        [HttpPost("process-document")]
        public async Task<IActionResult> ProcessDocument(CancellationToken cancellationToken)
        {
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "KnowledgeBase");
            await _chunkingService.ProcessFolder(folderPath, cancellationToken);
            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Document processing completed successfully.",
                Data = null
            });
        }
    }
}
