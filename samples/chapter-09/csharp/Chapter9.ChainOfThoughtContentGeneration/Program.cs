using Chapter9.ChainOfThoughtContentGeneration;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Plugins.Web;
using Microsoft.SemanticKernel.Plugins.Web.Google;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var kernel = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(
        configuration["LanguageModel:DeploymentName"]!,
        configuration["LanguageModel:Endpoint"]!,
        configuration["LanguageModel:ApiKey"]!
    ).Build();


kernel.Plugins.AddFromObject(new WebSearchEnginePlugin(new GoogleConnector(
    apiKey: configuration["Google:ApiKey"]!,
    searchEngineId: configuration["Google:SearchEngineId"]!)), "search_content");


var promptTemplate= kernel.CreateFunctionFromPromptYaml(
    EmbeddedResource.Read("Prompts.generate-content.yml"), 
    new HandlebarsPromptTemplateFactory());

var executionSettings = new AzureOpenAIPromptExecutionSettings
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};

var response = await promptTemplate.InvokeAsync(kernel, new KernelArguments(executionSettings)
{
    ["topic"] = "The importance of securing your AI agents in production"
});

Console.WriteLine(response.GetValue<String>());
