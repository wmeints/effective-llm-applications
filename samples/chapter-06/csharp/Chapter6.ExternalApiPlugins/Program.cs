using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Plugins.OpenApi;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var kernelBuilder = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(
        deploymentName: configuration["LanguageModel:DeploymentName"]!,
        endpoint: configuration["LanguageModel:Endpoint"]!,
        apiKey: configuration["LanguageModel:ApiKey"]!
    );

var kernel = kernelBuilder.Build();

await kernel.ImportPluginFromOpenApiAsync("time", new Uri("http://localhost:5019/openapi/v1.json"));
await kernel.ImportPluginFromOpenApiAsync("test", )

var arguments = new KernelArguments(new AzureOpenAIPromptExecutionSettings()
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
});

var response = await kernel.InvokePromptAsync("Can you tell me what time it is?", arguments);

Console.WriteLine(response.ToString());


