using Chapter10.IntelligentRequestRouting.Steps;
using Microsoft.SemanticKernel;

namespace Chapter10.IntelligentRequestRouting.Processes;


public class AnswerQuestionProcess
{
    private KernelProcess _process;

    public AnswerQuestionProcess()
    {
        var builder = new ProcessBuilder("AnswerQuestion");

        var basicQuestionStep = builder.AddStepFromType<HandleBasicPromptStep>();
        var complexQuestionStep = builder.AddStepFromType<HandleComplexPromptStep>();
        var routingStep = builder.AddStepFromType<RoutePromptStep>();

        builder.OnInputEvent("StartProcess").SendEventTo(new(routingStep));

        routingStep.OnEvent("HandleBasicPrompt")
            .SendEventTo(new ProcessFunctionTargetBuilder(basicQuestionStep));

        routingStep.OnEvent("HandleComplexPrompt")
            .SendEventTo(new ProcessFunctionTargetBuilder(complexQuestionStep));

        _process = builder.Build();
    }

    public async Task<string?> ExecuteAsync(Kernel kernel, string prompt)
    {
        var context = await _process.StartAsync(kernel, new KernelProcessEvent
        {
            Id = "StartProcess",
            Data = prompt
        });

        return kernel.Data["ResponseContent"] as string;
    }
}