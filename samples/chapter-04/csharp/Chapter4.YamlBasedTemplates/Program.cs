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

var promptTemplate = File.ReadAllText(Path.Join(Directory.GetCurrentDirectory(), "prompt.yaml"));
var prompt = kernel.CreateFunctionFromPromptYaml(promptTemplate, new HandlebarsPromptTemplateFactory());

var result = await kernel.InvokeAsync(prompt,
    arguments: new KernelArguments
    {
        ["dish"] = "pizza",
        ["ingredients"] = new List<string> { "pepperoni", "mozarella", "spinach" }
    });

Console.WriteLine(result);