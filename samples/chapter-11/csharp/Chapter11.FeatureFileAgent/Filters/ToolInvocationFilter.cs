using System.Text.Json;
using Microsoft.SemanticKernel;
using Spectre.Console;

namespace Chapter11.FeatureFileAgent.Filters;

public class ToolInvocationFilter : IFunctionInvocationFilter
{
    public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
    {
        var beforeToolRule = new Rule($"Tool: {context.Function.Name}");
        var afterToolRule = new Rule();

        AnsiConsole.Write(beforeToolRule);

        AnsiConsole.MarkupLine($"[bold green]Invoking function:[/] {context.Function.Name}");

        AnsiConsole.MarkupLine($"[bold blue]Arguments:[/] {JsonSerializer.Serialize(
            context.Arguments, new JsonSerializerOptions { WriteIndented = true })}");

        await next(context);

        var result = context.Result.GetValue<string>();
        AnsiConsole.MarkupLine($"[bold green]Function result:[/] {result}");

        AnsiConsole.Write(afterToolRule);
        AnsiConsole.WriteLine();
    }
}