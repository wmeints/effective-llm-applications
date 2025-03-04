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

var outputTool = new OutputTool();
var outputToolPlugin = kernel.Plugins.AddFromObject(outputTool);

var settings = new AzureOpenAIPromptExecutionSettings
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Required(
        [outputToolPlugin.First(x => x.Name == "CreateUserStory")])
};

var response = await kernel.InvokePromptAsync(
    "Generate a scenario with given, when, then statements for the " +
    "following user story: As I user I want to be able to chat to " +
    "customer support",
    new KernelArguments(settings)
);

Console.WriteLine(outputTool.Title);

foreach (var step in outputTool.Steps)
{
    Console.WriteLine(step);
}
