﻿using Chapter10.BasicProcess.Steps;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Process.Tools;

namespace Chapter10.BasicProcess.Processes;

public class GreetingProcess
{
    private KernelProcess _process;

    public GreetingProcess()
    {
        var processBuilder = new ProcessBuilder("GreetingProcess");

        var getNameStep = processBuilder.AddStepFromType<GetNameStep>();
        var generateGreetingStep = processBuilder.AddStepFromType<GenerateGreetingStep>();

        processBuilder.OnInputEvent("StartProcess").SendEventTo(new(getNameStep));

        getNameStep.OnFunctionResult()
            .SendEventTo(new ProcessFunctionTargetBuilder(generateGreetingStep));

        _process = processBuilder.Build();
    }

    public async Task<string> StartAsync(Kernel kernel)
    {
        await _process.StartAsync(kernel, new KernelProcessEvent { Id = "StartProcess" });
        var result = kernel.Data["GreetingMessage"] as string;

        return result!;
    }

    public string ToMermaid()
    {
        return _process.ToMermaid();
    }
}