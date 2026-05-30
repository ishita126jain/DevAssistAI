using DevAssistAI.Common;
using DevAssistAI.Data;
using DevAssistAI.MCP.Contact;
using DevAssistAI.MCP.Operation;
using DevAssistAI.Middleware;
using DevAssistAI.Repository.Contract;
using DevAssistAI.Repository.Operation;
using DevAssistAI.Service.Contract;
using DevAssistAI.Service.Operation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpClient(
    "OllamaClient",
    client =>
    {
        client.Timeout = TimeSpan.FromMinutes(10);
    });
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddScoped<IAIService, AIService>();
builder.Services.AddSingleton<IChatMemoryService, ChatMemoryService>();
builder.Services.AddSingleton<IVectorStoreService, VectorStoreService>();
builder.Services.AddSingleton<IMCPTool, SQLTool>();
builder.Services.AddSingleton<IMCPRouterService, MCPRouterSevice>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IProductionAIService, ProductionAIService>();
builder.Services.AddScoped<IChunkingService, ChunkingService>();
builder.Services.AddScoped<IDocumentChunkRepository, DocumentChunkRepository>();
builder.Services.AddScoped<IRAGService, RAGService>();
builder.Services.AddScoped<IEmbeddingService, EmbeddingService>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    );
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        List<string> errors = context.ModelState
            .Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

        ApiResponse<List<string>> response = new ApiResponse<List<string>>
            {
                Success = false,
                Message = "Validation failed",
                Data = errors
            };

        return new BadRequestObjectResult(response);
    };
});

var app = builder.Build();

app.UseCors("AllowAll");

app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
