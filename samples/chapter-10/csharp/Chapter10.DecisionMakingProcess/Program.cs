using Chapter10.DecisionMakingProcess.Processes;
using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddKernel();

var app = builder.Build();

app.MapGet("/decision", async (Kernel kernel) =>
{
    var process = new RandomDecisionMakingProcess();
    return await process.StartAsync(kernel);
});

app.Run();
