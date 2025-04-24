using System.Text.Json;
using Chapter10.ComplexProcess.Data;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;

namespace Chapter10.SimpleProcess.Steps;

public class CreateOutlineStep() : KernelProcessStep
{
    [KernelFunction]
    public async Task<InitialArticleStructure> CreateOutlineAsync(Kernel kernel, ResearchTopicResult researchTopicResult)
    {
        var promptTemplate = kernel.CreateFunctionFromPromptYaml(
            EmbeddedResource.Read("Prompts.create-outline.yml"),
            new HandlebarsPromptTemplateFactory());

        var promptExecutionSettings = new AzureOpenAIPromptExecutionSettings()
        {
            ResponseFormat = typeof(ArticleOutline),
            MaxTokens = 5000,
        };

        var result = await promptTemplate.InvokeAsync(kernel, new KernelArguments(promptExecutionSettings)
        {
            ["topic"] = researchTopicResult.Topic,
            ["searchResults"] = researchTopicResult.SearchResults
        });

        var outline = JsonSerializer.Deserialize<ArticleOutline>(result.GetValue<String>()!);

        if (outline is null)
        {
            throw new InvalidOperationException(
                "Can't deserialize outline from prompt response");
        }
        
        return new InitialArticleStructure(
            researchTopicResult.Topic,
            outline.Title,
            outline.Sections);
    }
}