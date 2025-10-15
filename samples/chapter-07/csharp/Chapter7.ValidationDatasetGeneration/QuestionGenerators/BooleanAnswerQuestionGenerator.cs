using Chapter7.ValidationDatasetGeneration.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;

namespace Chapter7.ValidationDatasetGeneration.QuestionGenerators;

public class BooleanAnswerQuestionGenerator(Kernel kernel, ILogger logger) : QuestionGeneratorBase(kernel, logger)
{
    protected override KernelFunction GetPromptTemplate(Kernel kernel)
    {
        return kernel.CreateFunctionFromPromptYaml(
            EmbeddedResource.Read("Prompts.BooleanAnswerQuestion.yaml"),
            new HandlebarsPromptTemplateFactory()
            {
                AllowDangerouslySetContent = true
            });
    }
}