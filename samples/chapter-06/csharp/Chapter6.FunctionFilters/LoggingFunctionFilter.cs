using Microsoft.SemanticKernel;

namespace Chapter6.FunctionFilters;

public class LoggingFunctionFilter : IFunctionInvocationFilter
{
    public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
    {
        Console.WriteLine($"Invoking {context.Function.Name}");

        foreach (var param in context.Arguments)
        {
            Console.WriteLine($"Parameter '{param.Key}' has value '{param.Value}");
        }
        
        await next(context);
        
        Console.WriteLine($"Done invoking {context.Function.Name}");
    }
}