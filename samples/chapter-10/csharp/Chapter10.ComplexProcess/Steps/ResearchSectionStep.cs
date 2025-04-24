using System.Text.Json;
using Chapter10.ComplexProcess.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Plugins.Web;
using Microsoft.SemanticKernel.Plugins.Web.Google;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;

namespace Chapter10.SimpleProcess.Steps;

public class ResearchSectionStep : KernelProcessStep
{
    private readonly WebSearchEnginePlugin _webSearchEnginePlugin;

    public ResearchSectionStep(IConfiguration configuration, ILogger<ResearchSectionStep> logger)
    {
        _webSearchEnginePlugin = new WebSearchEnginePlugin(new GoogleConnector(
            apiKey: configuration["Google:ApiKey"]!,
            searchEngineId: configuration["Google:SearchEngineId"]!));
    }

    [KernelFunction]
    public async Task<SectionResearch> ResearchSectionContentAsync(Kernel kernel, ResearchSectionInput input)
    {
        var researchQuery = await GenerateResearchQueryAsync(kernel, input.Topic, input.SectionTitle);
        var searchResults = await _webSearchEnginePlugin.SearchAsync(researchQuery);

        return new SectionResearch(input.SectionTitle, researchQuery, searchResults);
    }

    private async Task<string> GenerateResearchQueryAsync(Kernel kernel, string topic, string sectionTitle)
    {
        var promptTemplate = kernel.CreateFunctionFromPromptYaml(
            EmbeddedResource.Read("Prompts.create-research-query.yml"),
            new HandlebarsPromptTemplateFactory());

        var promptExecutionSettings = new AzureOpenAIPromptExecutionSettings()
        {
            ResponseFormat = typeof(ResearchQuestion),
        };

        var response = await promptTemplate.InvokeAsync(kernel, new KernelArguments(promptExecutionSettings)
        {
            ["topic"] = topic,
            ["sectionTitle"] = sectionTitle,
        });

        var researchQuestion = JsonSerializer.Deserialize<ResearchQuestion>(response.GetValue<string>()!);

        if (researchQuestion is null)
        {
            throw new InvalidOperationException("Couldn't deserialize the research question");
        }
        
        return researchQuestion.SearchQuery;
    }
}