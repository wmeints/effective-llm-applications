using Chapter10.ComplexProcess.Data;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;

namespace Chapter10.SimpleProcess.Steps;

public class WriteSectionStep(ILogger<WriteSectionStep> logger): KernelProcessStep<GeneratedContentState>
{
    private GeneratedContentState _state;
    
    public override ValueTask ActivateAsync(KernelProcessStepState<GeneratedContentState> state)
    {
        _state = state.State!;
        return base.ActivateAsync(state);
    }

    [KernelFunction]
    public async Task<SectionContent> GenerateSectionContentAsync(Kernel kernel, SectionResearch sectionResearch)
    {
        logger.LogInformation("Writing section content for section {SectionTitle}", sectionResearch.Section);
        
        var promptTemplate = kernel.CreateFunctionFromPromptYaml(
            EmbeddedResource.Read("Prompts.write-section.yml"), 
            new HandlebarsPromptTemplateFactory());

        var response = await promptTemplate.InvokeAsync(kernel, new KernelArguments()
        {
            ["sectionTitle"] = sectionResearch.Section,
            ["searchResults"] = sectionResearch.SearchResults,
            ["topic"] = _state.Topic,
            ["query"] = sectionResearch.ResearchQuery,
        });

        return new SectionContent(sectionResearch.Section, response.GetValue<string>()!);
    }
}