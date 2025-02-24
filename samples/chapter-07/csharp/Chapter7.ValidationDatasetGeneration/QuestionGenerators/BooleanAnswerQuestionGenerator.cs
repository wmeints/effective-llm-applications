using System.Text.Json;
using Chapter7.ContentPreprocessing.ProcessingSteps;
using Chapter7.ContentPreprocessing.Shared;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;

namespace Chapter7.ContentPreprocessing.QuestionGenerators;

public class BooleanAnswerQuestionGenerator(Kernel kernel): IQuestionGenerator
{
    private KernelFunction _prompt =  kernel.CreateFunctionFromPromptYaml(
        EmbeddedResource.Read("Prompts.BooleanAnswerQuestion.yaml"), new HandlebarsPromptTemplateFactory());

    public async IAsyncEnumerable<QuestionAnswerPair> GenerateQuestionsAsync(string content, int numberOfQuestions)
    {
        var promptExecutionSettings = new OpenAIPromptExecutionSettings
        {
            ResponseFormat = typeof(QuestionGeneratorResult)
        };

        var promptExecution = await _prompt.InvokeAsync(kernel, new KernelArguments(promptExecutionSettings)
        {
            ["context"] = content,
            ["count"] = numberOfQuestions
        });

        var responseData = JsonSerializer.Deserialize<QuestionGeneratorResult>(
            promptExecution.GetValue<string>()!);
        
        foreach(var item in responseData.QuestionAnswerPairs)
        {
            yield return item;
        }
    }
}