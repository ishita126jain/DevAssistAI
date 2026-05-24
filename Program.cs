using DevAssistAI.MCP.Contact;
using DevAssistAI.MCP.Operation;
using DevAssistAI.Service.Contract;
using DevAssistAI.Service.Operation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpClient();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddScoped<IAIService, AIService>();
builder.Services.AddSingleton<IChatMemoryService, ChatMemoryService>();
builder.Services.AddSingleton<IVectorStoreService, VectorStoreService>();
builder.Services.AddSingleton<IMCPTool, SQLTool>();
builder.Services.AddSingleton<IMCPRouterService, MCPRouterSevice>();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
