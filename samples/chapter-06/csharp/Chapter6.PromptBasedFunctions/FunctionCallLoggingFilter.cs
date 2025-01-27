using Microsoft.SemanticKernel;

public class FunctionCallLoggingFilter : IFunctionInvocationFilter
{
    public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
    {
        Console.WriteLine($"Invoking function {context.Function.Name}");

        await next(context);
    }
}