using Microsoft.SemanticKernel;

namespace Chapter10.DecisionMakingProcess.Steps;

public class HandleLowOutcomeStep: KernelProcessStep
{
    [KernelFunction]
    public void HandleLowOutcome(Kernel kernel, int randomValue)
    {
        kernel.Data["DecisionOutcome"] = "Low";
        kernel.Data["OriginalValue"] = randomValue;
    }
}