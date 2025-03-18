using System.Text.Json;
using Chapter9.PromptChainContentGeneration.Shared;
using Microsoft.SemanticKernel;

namespace Chapter9.PromptChainContentGeneration.Steps;

public class ResearchSectionStep
{
    private readonly KernelFunction _generateQuestionPromptTemplate;
    private readonly Kernel _kernel;
    private readonly WebSearchEnginePlugin _webSearchPlugin;

    public ResearchSectionStep(Kernel kernel, IConfiguration configuration)
    {
        _kernel = kernel;

        _generateQuestionPromptTemplate = kernel.CreateFunctionFromPromptYaml(
            EmbeddedResource.Read("Prompts.research-section.yml"),
            new HandlebarsPromptTemplateFactory());

        _webSearchPlugin = new WebSearchEnginePlugin(new GoogleConnector(
            apiKey: configuration["Google:ApiKey"]!,
            searchEngineId: configuration["Google:SearchEngineId"]!));
    }

    public async Task<ResearchSectionResult> InvokeAsync(string topic, string sectionTitle)
    {
        var searchQuery = await GenerateSearchQueryAsync(topic, sectionTitle);
        var searchResults = await _webSearchPlugin.SearchAsync(searchQuery);

        return new ResearchSectionResult(searchQuery, searchResults);
    }

    private async Task<string> GenerateSearchQueryAsync(string topic, string sectionTitle)
    {
        var promptExecutionSettings = new OpenAIPromptExecutionSettings
        {
            ResponseFormat = typeof(GenerateSearchQueryResult)
        };

        var response = await _generateQuestionPromptTemplate.InvokeAsync(
            _kernel, new KernelArguments(promptExecutionSettings)
            {
                ["topic"] = topic,
                ["sectionTitle"] = sectionTitle
            });

        string responseData = response.GetValue<string>()!;
        var searchQueryResult = JsonSerializer.Deserialize<GenerateSearchQueryResult>(responseData);

        if (searchQueryResult is null)
        {
            throw new InvalidOperationException("Failed to generate search query");
        }

        Console.WriteLine(searchQueryResult.SearchQuery);

        return searchQueryResult.SearchQuery;
    }
}
