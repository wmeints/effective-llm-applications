using System.Diagnostics.CodeAnalysis;
using Microsoft.SemanticKernel;

namespace Chapter10.BasicProcess.Steps;

public class GetNameStep: KernelProcessStep
{
    [KernelFunction]
    public string GetName()
    {
        return "Willem";
    }    
}