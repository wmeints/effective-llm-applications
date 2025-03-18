using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;

namespace Chapter9.PromptChainContentGeneration.Tests;

public class TestObjectFactory
{
    public static IConfiguration GetTestConfiguration()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<TestObjectFactory>()
            .Build();

        return configuration;
    }

    public static Kernel GetKernel(IConfiguration configuration)
    {
        var kernel = Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(
                configuration["LanguageModel:DeploymentName"]!,
                configuration["LanguageModel:Endpoint"]!,
                configuration["LanguageModel:ApiKey"]!
            ).Build();

        return kernel;
    }
}