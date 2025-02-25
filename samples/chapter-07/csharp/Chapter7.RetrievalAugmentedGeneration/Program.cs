using Chapter7.RetrievalAugmentedGeneration.Services;
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

builder.Services.AddSingleton<ContentIndexer>();
builder.Services.AddSingleton<QuestionAnsweringTool>();

var app = builder.Build();

app.MapGet("/answer", async ([FromServices] QuestionAnsweringTool tool, [FromQuery] string question) =>
{
    var result = await tool.AnswerAsync(question);
    return result.Response;
});

var scope = app.Services.CreateScope();
var indexer = scope.ServiceProvider.GetRequiredService<ContentIndexer>();

// await indexer.ProcessContentAsync();

await app.RunAsync();
