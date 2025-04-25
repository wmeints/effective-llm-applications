using Microsoft.SemanticKernel;

namespace Chapter10.DecisionMakingProcess.Steps;

public class GenerateRandomNumberStep: KernelProcessStep
{
    private readonly Random _random = new Random();
    
    [KernelFunction]
    public int GenerateRandomNumber()
    {
        return _random.Next(1, 21);
    }
}