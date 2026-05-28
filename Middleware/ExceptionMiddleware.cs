using DevAssistAI.Common;
using DevAssistAI.Exceptions;
using System.Text.Json;

namespace DevAssistAI.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {

                ILogger<ExceptionMiddleware> logger = context.RequestServices.GetRequiredService<ILogger<ExceptionMiddleware>>();

                logger.LogError(ex, "Unhandled exception occurred.");

                int statusCode = 500;
                if (ex is NotFoundException)
                {
                    statusCode = 404;
                }
                if(ex is TaskCanceledException)
                {
                    statusCode = 408;
                }

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = statusCode;
                
                ApiResponse<string> response = new ApiResponse<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };

                string json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);
            }
        }
    }
}

