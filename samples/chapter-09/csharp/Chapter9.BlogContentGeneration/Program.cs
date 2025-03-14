using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Web;
using Microsoft.SemanticKernel.Plugins.Web.Google;

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
    searchEngineId: configuration["Google:SearchEngineId"]!)));
