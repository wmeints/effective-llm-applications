using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;

namespace Chapter7.ContentPreprocessing.Shared;

public class KernelFactory
{
    public static Kernel CreateKernel(IConfiguration configuration)
    {
        var languageModelConfig = configuration
            .GetSection("LanguageModel")
            .Get<LanguageModelConfiguration>();

        if (languageModelConfig == null)
        {
            throw new InvalidOperationException(
                "Can't find configuration for the language model. " +
                "Please make sure you configured the required user-secrets");
        }
        
        var kernel = Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(
                deploymentName: languageModelConfig.ChatCompletionModel, 
                endpoint: languageModelConfig.Endpoint,
                apiKey: languageModelConfig.ApiKey
            )
            .AddAzureOpenAITextEmbeddingGeneration(
                deploymentName: languageModelConfig.TextEmbeddingModel, 
                endpoint: languageModelConfig.Endpoint,
                apiKey: languageModelConfig.ApiKey
            )
            .Build();

        return kernel;
    }
}