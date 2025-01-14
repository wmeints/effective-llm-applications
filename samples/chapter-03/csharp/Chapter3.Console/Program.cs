using Microsoft.SemanticKernel;
using Azure.AI.OpenAI;
using Azure.Identity;
using Chapter1.Console;

IKernelBuilder kernelBuilder = Kernel.CreateBuilder();

var apiClient = new AzureOpenAIClient(
    new Uri("https://<your-endpoint>"),
    new DefaultAzureCredential());

kernelBuilder.AddAzureOpenAIChatCompletion(
    deploymentName: "gpt-4o",
    azureOpenAIClient: apiClient);

// ALTERNATIVELY: Use the OpenAI API directly.

// var apiClient = new OpenAIClient(
//     Environment.GetEnvironmentVariable("OPENAI_API_KEY"));

// kernelBuilder.AddOpenAIChatCompletion("gpt-4o", apiClient);

Kernel kernel = kernelBuilder.Build();

ProductNameGenerator generator = new ProductNameGenerator(kernel);
string productNames = await generator.GenerateProductNames();

Console.WriteLine(productNames);