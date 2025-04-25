using Chapter10.BasicProcess.Processes;
using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/greeting", async (Kernel kernel) =>
{
    var process = new GreetingProcess();
    return await process.StartAsync(kernel);
});

var process = new GreetingProcess();
Console.WriteLine(process.ToMermaid());

app.Run();
