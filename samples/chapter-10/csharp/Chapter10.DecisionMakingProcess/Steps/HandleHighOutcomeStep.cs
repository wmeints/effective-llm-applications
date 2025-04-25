using Microsoft.SemanticKernel;

namespace Chapter10.DecisionMakingProcess.Steps;

public class HandleHighOutcomeStep: KernelProcessStep
{
    [KernelFunction]
    public void HandleHighOutcome(Kernel kernel, int randomValue)
    {
        kernel.Data["DecisionOutcome"] = "High";
        kernel.Data["OriginalValue"] = randomValue;
    }
}