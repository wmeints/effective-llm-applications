using Chapter10.ComplexProcess.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Web;
using Microsoft.SemanticKernel.Plugins.Web.Google;

namespace Chapter10.SimpleProcess.Steps;

public class ResearchTopicStep : KernelProcessStep
{
    private readonly WebSearchEnginePlugin _webSearchEnginePlugin;

    public ResearchTopicStep(IConfiguration configuration, ILogger<ResearchTopicStep> logger)
    {
        _webSearchEnginePlugin = new WebSearchEnginePlugin(new GoogleConnector(
            apiKey: configuration["Google:ApiKey"]!,
            searchEngineId: configuration["Google:SearchEngineId"]!));
    }

    [KernelFunction]
    public async Task<ResearchTopicResult> ResearchContentAsync(string topic)
    {
        var searchResults = await _webSearchEnginePlugin.SearchAsync(topic);
        return new ResearchTopicResult(topic, searchResults);
    }
}