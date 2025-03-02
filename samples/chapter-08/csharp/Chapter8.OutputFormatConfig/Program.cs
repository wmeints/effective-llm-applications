using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var kernel = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(
        deploymentName: configuration["LanguageModel:DeploymentName"]!,
        endpoint: configuration["LanguageModel:Endpoint"]!,
        apiKey: configuration["LanguageModel:ApiKey"]!
    )
    .Build();

var settings = new AzureOpenAIPromptExecutionSettings
{
    ResponseFormat = typeof(ScenarioResult)
};

var response = await kernel.InvokePromptAsync(
    "Generate a scenario with given, when, then statements for the " +
    "following user story: As I user I want to be able to chat to " +
    "customer support",
    new KernelArguments(settings)
);

var responseData = JsonSerializer.Deserialize<ScenarioResult>(response.GetValue<string>()!);

Console.WriteLine(responseData?.Title);
Console.WriteLine(responseData?.Description);

foreach (var step in responseData!.Steps)
{
    Console.WriteLine(step);
}
