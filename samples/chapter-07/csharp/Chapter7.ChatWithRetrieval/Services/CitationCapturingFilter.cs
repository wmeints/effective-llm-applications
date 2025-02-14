using Chapter7.ChatWithRetrieval.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Data;

namespace Chapter7.ChatWithRetrieval.Services;

public class CitationCapturingFilter : IFunctionInvocationFilter
{
    public List<TextSearchResult> Captures { get; } = new();

    public async Task OnFunctionInvocationAsync(
        FunctionInvocationContext context,
        Func<FunctionInvocationContext, Task> next)
    {
        await next(context);

        if (context.Function.PluginName == "SearchPlugin")
        {
            var results = context.Result.GetValue<List<TextSearchResult>>()!;
            Captures.AddRange(results);
        }
    }
}