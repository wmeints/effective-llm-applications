using Microsoft.SemanticKernel;

namespace Chapter4.PromptFilters;

public class PIIFilter : IPromptRenderFilter
{
    public async Task OnPromptRenderAsync(PromptRenderContext context, Func<PromptRenderContext, Task> next)
    {
        // This function is called when the prompt is rendered. This is where
        // we can filter the contents of the prompt before it's submitted.
        await next(context);
        
        //TODO: Filter the prompt contents
        //context.RenderedPrompt = "My prompt content";
    }
}