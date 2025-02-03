using System.Security.Cryptography;
using Chapter6.FunctionFilters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var kernelBuilder = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(
        deploymentName: configuration["LanguageModel:DeploymentName"]!,
        endpoint: configuration["LanguageModel:Endpoint"]!,
        apiKey: configuration["LanguageModel:ApiKey"]!
    );

kernelBuilder.Plugins.AddFromObject(new TimePlugin());
kernelBuilder.Services.AddTransient<IFunctionInvocationFilter, LoggingFunctionFilter>();

var arguments = new KernelArguments(new AzureOpenAIPromptExecutionSettings
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
});

var kernel = kernelBuilder.Build();
var response = await kernel.InvokePromptAsync("Can you tell me what time it is?", arguments);

Console.WriteLine(response.ToString());
