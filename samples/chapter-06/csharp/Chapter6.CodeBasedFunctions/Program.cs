using Chapter6.CodeBasedFunctions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var kernelBuilder = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(
        deploymentName: configuration["LanguageModel:DeploymentName"]!,
        endpoint: configuration["LanguageModel:Endpoint"]!,
        apiKey: configuration["LanguageModel:ApiKey"]!
    );

kernelBuilder.Plugins.AddFromType<TimePlugin>();

var kernel = kernelBuilder.Build();

var chatCompletionService =
    kernel.Services.GetRequiredService<IChatCompletionService>();

var history = new ChatHistory();
history.AddSystemMessage("You're a digital assistant");
history.AddUserMessage("What time is it?");

var executionSettings = new AzureOpenAIPromptExecutionSettings
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.None()
};

var response = await chatCompletionService.GetChatMessageContentAsync(
    history, executionSettings, kernel);

Console.WriteLine(response.ToString());