using Chapter10.ComplexProcess.Data;

namespace Chapter10.SimpleProcess.Steps;

public class GenerateContentStep: KernelProcessStep<GeneratedContentState>
{
    private GeneratedContentState _state;
    
    public override ValueTask ActivateAsync(KernelProcessStepState<GeneratedContentState> state)
    {
        _state = state.State!;
        return base.ActivateAsync(state);
    }

    [KernelFunction]
    public async Task StartGenerateContentAsync(KernelProcessStepContext context, InitialArticleStructure data)
    {
        if (_state.Status == ContentGenerationStatus.Pending)
        {
            _state.PendingSections = new Queue<string>(data.Sections);
            _state.Topic = data.Topic;
        }
        
        if (_state.PendingSections.Count > 0)
        {
            _state.Status = ContentGenerationStatus.GeneratingSections;
            
            var currentSection = _state.PendingSections.Dequeue();
            
            await context.EmitEventAsync("ResearchSection", data: new ResearchSectionInput(
                _state.Topic, currentSection));
        }
        else
        {
            _state.Status = ContentGenerationStatus.Completed;
            
            var finalArticleContent = new FinalizeArticleInput(_state.Topic, _state.Title, _state.GeneratedSections);
            await context.EmitEventAsync("FinalizeArticle", data: finalArticleContent);
        }
    }

    [KernelFunction]
    public async Task ContinueGenerateContentAsync(KernelProcessStepContext context, SectionContent sectionContent)
    {
        _state.GeneratedSections.Add(sectionContent);
        
        if (_state.PendingSections.Count > 0)
        {
            _state.Status = ContentGenerationStatus.GeneratingSections;
            
            var currentSection = _state.PendingSections.Dequeue();
            
            await context.EmitEventAsync("ResearchSection", data: new ResearchSectionInput(
                _state.Topic, currentSection));
        }
        else
        {
            _state.Status = ContentGenerationStatus.Completed;
            
            var finalArticleContent = new FinalizeArticleInput(_state.Topic, _state.Title, _state.GeneratedSections);
            await context.EmitEventAsync("FinalizeArticle", data: finalArticleContent);
        }
    }
}