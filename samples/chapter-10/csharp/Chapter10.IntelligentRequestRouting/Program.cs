using Chapter10.IntelligentRequestRouting.Processes;
using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddKernel()
    .AddAzureOpenAIChatCompletion(
        builder.Configuration["LanguageModel:ComplexCompletionModel"]!,
        builder.Configuration["LanguageModel:Endpoint"]!,
        builder.Configuration["LanguageModel:ApiKey"]!,
        serviceId: "complexPrompts")
    .AddAzureOpenAIChatCompletion(
        builder.Configuration["LanguageModel:BasicCompletionModel"]!,
        builder.Configuration["LanguageModel:Endpoint"]!,
        builder.Configuration["LanguageModel:ApiKey"]!,
        serviceId: "basicPrompts");

var app = builder.Build();

app.MapPost("/answer", async (string q, Kernel kernel) =>
{
    var process = new AnswerQuestionProcess();
    var answer = await process.ExecuteAsync(kernel, q);

    return answer;
});

app.Run();
