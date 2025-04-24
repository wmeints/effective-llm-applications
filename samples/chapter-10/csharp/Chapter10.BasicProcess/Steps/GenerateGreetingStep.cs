using Microsoft.SemanticKernel;

namespace Chapter10.BasicProcess.Steps;

public class GenerateGreetingStep: KernelProcessStep
{
    [KernelFunction]
    public void GenerateGreeting(string name)
    {
        Console.WriteLine("$Hello, ${name}!");
    }
}