using System.ComponentModel.DataAnnotations;

namespace DevAssistAI.DTO
{
    public class ProductionChatRequest
    {
        public Guid? SessionId { get; set; }

        [Required]
        public string Prompt { get; set; } = string.Empty;
    }
}
