using Microsoft.SemanticKernel.Process.Tools;

namespace Chapter10.SimpleProcess.Processes;

public class CreateArticleProcess
{
    private readonly KernelProcess _process;
    private readonly Kernel _kernel;

    public CreateArticleProcess(Kernel kernel)
    {
        var processBuilder = new ProcessBuilder("CreateArticle");

        var researchTopicStep = processBuilder.AddStepFromType<ResearchTopicStep>();
        var createOutlineStep = processBuilder.AddStepFromType<CreateOutlineStep>();
        var generateContentStep = processBuilder.AddStepFromType<GenerateContentStep>();
        var researchSectionStep = processBuilder.AddStepFromType<ResearchSectionStep>();
        var writeSectionStep = processBuilder.AddStepFromType<WriteSectionStep>();
        var finalizeArticleStep = processBuilder.AddStepFromType<FinalizeArticleStep>();

        processBuilder
            .OnInputEvent("CreateArticle")
            .SendEventTo(new(researchTopicStep));

        researchTopicStep
            .OnFunctionResult()
            .SendEventTo(new ProcessFunctionTargetBuilder(createOutlineStep));

        createOutlineStep
            .OnFunctionResult()
            .SendEventTo(new ProcessFunctionTargetBuilder(generateContentStep, functionName: "StartGenerateContent"));

        generateContentStep.OnEvent("ResearchSection")
            .SendEventTo(new ProcessFunctionTargetBuilder(researchSectionStep));

        generateContentStep.OnEvent("FinalizeArticle")
            .SendEventTo(new ProcessFunctionTargetBuilder(finalizeArticleStep));

        researchSectionStep
            .OnFunctionResult()
            .SendEventTo(new ProcessFunctionTargetBuilder(writeSectionStep));

        writeSectionStep
            .OnFunctionResult()
            .SendEventTo(new ProcessFunctionTargetBuilder(
                step: generateContentStep,
                functionName: "ContinueGenerateContent"));

        _kernel = kernel;
        _process = processBuilder.Build();
    }

    public async Task StartAsync(string topic)
    {
        await _process.StartAsync(_kernel, new KernelProcessEvent { Id = "CreateArticle", Data = topic });
    }

    public string ToMermaid()
    {
        return _process.ToMermaid();
    }
}