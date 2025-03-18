namespace Chapter9.PromptChainContentGeneration.Steps;

public class ResearchContentStep
{
    private readonly WebSearchEnginePlugin _webSearchPlugin;

    public ResearchContentStep(IConfiguration configuration)
    {
        _webSearchPlugin = new WebSearchEnginePlugin(new GoogleConnector(
            apiKey: configuration["Google:ApiKey"]!,
            searchEngineId: configuration["Google:SearchEngineId"]!));
    }

    public async Task<ResearchContentResult> InvokeAsync(string topic)
    {
        var searchResults = await _webSearchPlugin.SearchAsync(topic);
        return new ResearchContentResult(searchResults);
    }
}

public record ResearchContentResult(string SearchResults);
