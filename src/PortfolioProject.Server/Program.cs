using PortfolioProject.Application;
using PortfolioProject.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();
builder.AddRedisClientBuilder("cache")
    .WithOutputCache();

// Add application and infrastructure services.
builder.Services.AddApplicationServices();
builder.AddInfrastructureServices();

// Add services to the container.
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseOutputCache();

var api = app.MapGroup("/api");
// Endpoint groups will be mapped here as features are added.

api.MapGet("/start", () =>
{
    return "it's working";
});

app.MapDefaultEndpoints();

app.UseFileServer();

await app.RunAsync();
