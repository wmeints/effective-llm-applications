using Microsoft.SemanticKernel;

namespace Chapter10.BasicProcess.Steps;

public class GenerateGreetingStep: KernelProcessStep
{
    [KernelFunction]
    public void GenerateGreeting(Kernel kernel, string name)
    {
        kernel.Data["GreetingMessage"] = $"Hello, {name}";
    }
}