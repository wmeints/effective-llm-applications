using Microsoft.SemanticKernel;

namespace Chapter4.PromptFilters;

public class PIIFilter : IPromptRenderFilter
{
    public async Task OnPromptRenderAsync(PromptRenderContext context, Func<PromptRenderContext, Task> next)
    {
        // This function is called when the prompt is rendered. This is where
        // we can filter the contents of the prompt before it's submitted.

        var renderedPrompt = context.RenderedPrompt;

        //TODO: Filter the prompt contents

        // Replace the original prompt with the filtered prompt.
        context.RenderedPrompt = renderedPrompt;
        await next(context);
    }
}