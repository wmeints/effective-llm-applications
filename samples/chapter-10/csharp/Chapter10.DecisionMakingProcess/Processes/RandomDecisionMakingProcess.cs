using Chapter10.DecisionMakingProcess.Models;
using Chapter10.DecisionMakingProcess.Steps;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Process.Tools;

namespace Chapter10.DecisionMakingProcess.Processes;

public class RandomDecisionMakingProcess
{
    private readonly KernelProcess _process;

    public RandomDecisionMakingProcess()
    {
        var processBuilder = new ProcessBuilder("RandomDecisionMaking");
        
        var generateRandomNumberStep = processBuilder.AddStepFromType<GenerateRandomNumberStep>();
        var makeDecisionStep = processBuilder.AddStepFromType<MakeDecisionStep>();
        var handleLowOutcomeStep = processBuilder.AddStepFromType<HandleLowOutcomeStep>();
        var handleHighOutcomeStep = processBuilder.AddStepFromType<HandleHighOutcomeStep>();
        
        processBuilder.OnInputEvent("StartProcess").SendEventTo(new(generateRandomNumberStep));
        generateRandomNumberStep.OnFunctionResult().SendEventTo(new(makeDecisionStep));
        
        makeDecisionStep.OnEvent("HighOutcome").SendEventTo(new(handleHighOutcomeStep));
        makeDecisionStep.OnEvent("LowOutcome").SendEventTo(new(handleLowOutcomeStep));
        
        _process = processBuilder.Build();
    }

    public async Task<ProcessOutcome> StartAsync(Kernel kernel)
    {
        await _process.StartAsync(kernel, new KernelProcessEvent { Id = "StartProcess" });
        
        var decisionOutcome = (string)kernel.Data["DecisionOutcome"]!;
        var originalValue = (int)kernel.Data["OriginalValue"]!;

        return new ProcessOutcome(decisionOutcome, originalValue);
    }

    public string ToMermaid()
    {
        return _process.ToMermaid();
    }
}