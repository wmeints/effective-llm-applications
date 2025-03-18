using System.ComponentModel;

namespace Chapter9.PromptChainContentGeneration.Steps;

public record CreateOutlineResult(
    [Description("The title of the article")] string Title,
    [Description("The top-level sections for the article")] List<string> Sections);