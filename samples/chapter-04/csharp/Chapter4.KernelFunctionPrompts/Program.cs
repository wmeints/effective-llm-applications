using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
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

var promptTemplate = File.ReadAllText(
    Path.Join(Directory.GetCurrentDirectory(), "prompt.txt")
);

var executionSettings = new AzureOpenAIPromptExecutionSettings
{
    MaxTokens = 1200,
    Temperature = 0.5,
    TopP = 1.0,
    FrequencyPenalty = 0.0,
    PresencePenalty = 0.0
};

var prompt = kernel.CreateFunctionFromPrompt(
    promptTemplate, templateFormat: "handlebars",
    promptTemplateFactory: new HandlebarsPromptTemplateFactory(),
    executionSettings: executionSettings);

var result = await kernel.InvokeAsync(prompt, new KernelArguments
{
    ["dish"] = "pizza",
    ["ingredients"] = new List<string>
    {
        "pepperoni",
        "mozzarella",
        "spinach"
    }
});

Console.WriteLine(result);