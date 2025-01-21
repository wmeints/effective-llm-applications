using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;

namespace Chapter5.PromptTestingBasics;

public class RecipeGenerationTests
{
    private readonly Kernel _kernel;
    private readonly KernelFunction _prompt;

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
    }

    [Theory]
    [Trait("Category", "LLM")]
    [InlineData("pizza", new string[] { "spinach", "mozzarella" })]
    public async Task TestGenerateRecipe_ContainsInstructions(string dish, string[] ingredients)
    {
        var result = await _prompt.InvokeAsync(_kernel, new KernelArguments
        {
            ["dish"] = dish,
            ["ingredients"] = ingredients
        });

        Assert.Contains("ingredients", result.ToString().ToLower());
        Assert.Contains("instructions", result.ToString().ToLower());
    }
}