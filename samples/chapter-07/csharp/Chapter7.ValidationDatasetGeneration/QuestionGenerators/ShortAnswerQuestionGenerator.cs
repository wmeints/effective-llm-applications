using System.ClientModel;
using System.Text.Json;
using Chapter7.ValidationDatasetGeneration.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;

namespace Chapter7.ValidationDatasetGeneration.QuestionGenerators;

public class ShortAnswerQuestionGenerator(Kernel kernel, ILogger logger) : QuestionGeneratorBase(kernel, logger)
{
    protected override KernelFunction GetPromptTemplate(Kernel kernel)
    {
        return kernel.CreateFunctionFromPromptYaml(
            EmbeddedResource.Read("Prompts.ShortAnswerQuestion.yaml"),
            new HandlebarsPromptTemplateFactory());
    }
}