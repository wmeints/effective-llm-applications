using Microsoft.SemanticKernel;

namespace Chapter4.PromptFilters;

public class PIIFilter: IFunctionInvocationFilter, IPromptRenderFilter
{
    public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
    {
        // This function is called when the prompt is executed using the LLM provider.
        // We can't modify the prompt at this point. We can only filter the outpput.
        
        await next(context);
        
        var output = context.Result.GetValue<string>();
        
        //TODO: Replace PII in the output

        // Create a new function result, based on the old one, but with the filtered value.
        context.Result = new FunctionResult(context.Result, output);
    }

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