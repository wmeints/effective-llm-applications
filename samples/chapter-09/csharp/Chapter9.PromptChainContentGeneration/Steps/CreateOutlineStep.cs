using Chapter9.PromptChainContentGeneration.Shared;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Chapter9.PromptChainContentGeneration.Steps;

public class CreateOutlineStep
{
    private readonly KernelFunction _promptTemplate;
    private readonly Kernel _kernel;

    public CreateOutlineStep(Kernel kernel)
    {
        _kernel = kernel;
        _promptTemplate = kernel.CreateFunctionFromPromptYaml(
            EmbeddedResource.Read("Prompts.create-outline.yml"),
            new HandlebarsPromptTemplateFactory());
    }

    public async Task<CreateOutlineResult> InvokeAsync(string topic, string searchResults)
    {
        if (string.IsNullOrWhiteSpace(topic))
        {
            throw new ArgumentException("Topic cannot be null or empty", nameof(topic));
        }

        var executionSettings = new OpenAIPromptExecutionSettings()
        {
            ResponseFormat = typeof(CreateOutlineResult)
        };

        var response = await _promptTemplate.InvokeAsync(_kernel, new KernelArguments(executionSettings)
        {
            ["topic"] = topic,
            ["searchResults"] = searchResults
        });

        var responseData = JsonSerializer.Deserialize<CreateOutlineResult>(response.GetValue<string>()!);

        if (responseData is null)
        {
            throw new InvalidOperationException("Failed to deserialize response");
        }

        return responseData;
    }
}