using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;

namespace Chapter7.ContentPreprocessing.Tests.QuestionGenerators;

public class TestObjectFactory
{
    public static IConfiguration CreateConfiguration()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<TestObjectFactory>()
            .Build();

        return configuration;
    }

    public static Kernel CreateKernel()
    {
        var configuration = CreateConfiguration();

        var kernel = Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(
                deploymentName: configuration["LanguageModel:DeploymentName"]!,
                endpoint: configuration["LanguageModel:Endpoint"]!,
                apiKey: configuration["LanguageModel:ApiKey"]!
            )
            .Build();

        return kernel;
    }
}