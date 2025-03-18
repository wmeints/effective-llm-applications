using System.ComponentModel;

namespace Chapter9.PromptChainContentGeneration.Steps;

public record GenerateSearchQueryResult(
    [Description("The search query I should ask Google")] string SearchQuery);