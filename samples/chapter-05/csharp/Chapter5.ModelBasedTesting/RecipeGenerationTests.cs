using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using System.Text.Json;

namespace Chapter5.ModelBasedTesting;

public class RecipeGenerationTests
{
    private readonly Kernel _kernel;
    private readonly KernelFunction _prompt;
    private readonly KernelFunction _testPrompt;

    public RecipeGenerationTests()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<RecipeGenerationTests>()
            .Build();

        _kernel = Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(
                configuration["LanguageModel:DeploymentName"]!,
                endpoint: configuration["LanguageModel:Endpoint"]!,
                apiKey: configuration["LanguageModel:ApiKey"]!
            ).Build();

        _prompt = _kernel.CreateFunctionFromPromptYaml(
            EmbeddedResource.Read("prompt.yaml"), new HandlebarsPromptTemplateFactory()
        );

        _testPrompt = _kernel.CreateFunctionFromPromptYaml(
            EmbeddedResource.Read("test_prompt.yaml"), new HandlebarsPromptTemplateFactory()
        );
    }

    [Fact]
    public async Task TestGenerateRecipe_GeneratesConsistentResults()
    {
        // First, generate a response for the input sample.
        var result = await _prompt.InvokeAsync(_kernel, new KernelArguments
        {
            ["dish"] = "pizza",
            ["ingredients"] = new string[] { "spinach", "mozzarella" }
        });

        // Next, use the generated response with the
        // test prompt to score the outcome
        // on a scale from 1 to 5. Note: we're using a
        // structured response format
        // so the output is parsable as JSON.
        var testExecutionSettings = new AzureOpenAIPromptExecutionSettings
        {
            ResponseFormat = typeof(EvaluationResult)
        };

        var testResult = await _testPrompt.InvokeAsync(_kernel,
            new KernelArguments(testExecutionSettings)
            {
                ["recipe"] = result.ToString()
            });

        var evaluationResult = JsonSerializer.Deserialize<EvaluationResult>(
            testResult.ToString())!;

        Assert.InRange(evaluationResult.Score, 3, 5);
    }
}

public class EvaluationResult
{
    public int Score { get; set; }
}
