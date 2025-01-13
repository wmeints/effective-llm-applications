using Azure.AI.OpenAI;
using Azure.Identity;
using Chapter1.WebApi;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient(sp => new AzureOpenAIClient(
    new Uri(builder.Configuration["AzureOpenAI:Endpoint"]!),
    new DefaultAzureCredential()));

var kernelBuilder = builder.Services.AddKernel()
    .AddAzureOpenAIChatCompletion(deploymentName: "gpt-4o");

// ALTERNATIVELY: Use the OpenAI API directly.

// builder.Services.AddTransient(sp => new OpenAIClient(builder.Configuration["OpenAI:ApiKey"]));

// var kernelBuilder = builder.Services.AddKernel()
//     .AddOpenAIChatCompletion("gpt-4o");

builder.Services.AddTransient<ProductNameGenerator>();

var app = builder.Build();

app.MapGet("/", (ProductNameGenerator productNameGenerator) => productNameGenerator.GenerateProductNames());

app.Run();
