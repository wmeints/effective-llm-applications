using Microsoft.SemanticKernel;

namespace Chapter10.DecisionMakingProcess.Steps;

public class MakeDecisionStep: KernelProcessStep
{
    [KernelFunction]
    public async Task MakeDecisionAsync(KernelProcessStepContext context, int randomValue)
    {
        if (randomValue > 10)
        {
            await context.EmitEventAsync("HighOutcome", randomValue);
        }
        else
        {
            await context.EmitEventAsync("LowOutcome", randomValue);
        }
    }
}