using System.Net.Sockets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;

// The following code adds configuration support to the application.
// You can securely set the configuration values in the user secrets store.
// Use the command dotnet user-secrets set "<key>" "<value>" to configure a secret value.

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddUserSecrets<Program>()
    .AddJsonFile("appsettings.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

var kernel = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(
        configuration["LanguageModel:DeploymentName"]!,
        endpoint: configuration["LanguageModel:Endpoint"]!,
        apiKey: configuration["LanguageModel:ApiKey"]!
    )
    .Build();

var executionSettings = new AzureOpenAIPromptExecutionSettings()
{
    Temperature = 0.7,
    PresencePenalty = 0.2,
    FrequencyPenalty = 0.4,
    TopP = 0.98,
};

var chatHistory = new ChatHistory();

chatHistory.AddSystemMessage("You're a digital chef help me cook. Your name is Flora.");
chatHistory.AddUserMessage("Hi, I'd like a nice recipe for a french style apple pie");

var chatCompletionService = kernel.Services.GetService<IChatCompletionService>();
var responseIterator = chatCompletionService!.GetStreamingChatMessageContentsAsync(chatHistory);

await foreach (var token in responseIterator)
{
    Console.Write(token.Content);
}