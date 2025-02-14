using Chapter7.ChatWithRetrieval.Models;
using Chapter7.ChatWithRetrieval.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Qdrant.Client;

var builder = WebApplication.CreateBuilder(args);

var kernelBuilder = builder.Services.AddKernel()
    .AddAzureOpenAIChatCompletion(
        deploymentName: builder.Configuration["LanguageModel:CompletionModel"]!,
        endpoint: builder.Configuration["LanguageModel:Endpoint"]!,
        apiKey: builder.Configuration["LanguageModel:ApiKey"]!
    )
    .AddAzureOpenAITextEmbeddingGeneration(
        deploymentName: builder.Configuration["LanguageModel:EmbeddingModel"]!,
        endpoint: builder.Configuration["LanguageModel:Endpoint"]!,
        apiKey: builder.Configuration["LanguageModel:ApiKey"]!
    );

builder.Services.AddSingleton<IVectorStore>(
    sp => new QdrantVectorStore(new QdrantClient("localhost")));

builder.Services.AddTransient<QuestionAnsweringBot>();
builder.Services.AddSingleton<ContentIndexer>();

var app = builder.Build();

app.MapPost("/chat", async ([FromServices] QuestionAnsweringBot completions, ChatInputForm form) =>
{
    return await completions.GenerateResponse(form.Prompt);
});

var scope = app.Services.CreateScope();
var indexer = scope.ServiceProvider.GetRequiredService<ContentIndexer>();

// await indexer.ProcessContentAsync();

await app.RunAsync();
