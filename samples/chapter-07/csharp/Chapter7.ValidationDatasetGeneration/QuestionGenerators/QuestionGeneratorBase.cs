using System.ClientModel;
using System.Net;
using System.Text.Json;
using Chapter7.ContentPreprocessing.QuestionGenerators;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace Chapter7.ValidationDatasetGeneration.QuestionGenerators;

public abstract class QuestionGeneratorBase : IQuestionGenerator
{
    private readonly Kernel _kernel;
    private readonly ILogger _logger;

    public QuestionGeneratorBase(Kernel kernel, ILogger logger)
    {
        _kernel = kernel;
        _logger = logger;
    }

    public async IAsyncEnumerable<QuestionAnswerPair> GenerateQuestionsAsync(string content, int numberOfQuestions)
    {
        List<QuestionAnswerPair> questionAnswerPairs;

        var promptTemplate = GetPromptTemplate(_kernel);

        var promptExecutionSettings = new OpenAIPromptExecutionSettings
        {
            ResponseFormat = typeof(QuestionGeneratorResult)
        };

        try
        {
            var promptExecution = await promptTemplate.InvokeAsync(
                _kernel,
                new KernelArguments(promptExecutionSettings)
                {
                    ["context"] = content,
                    ["count"] = numberOfQuestions
                });

            var responseData = JsonSerializer.Deserialize<QuestionGeneratorResult>(
                promptExecution.GetValue<string>()!);

            questionAnswerPairs = responseData!.QuestionAnswerPairs;
        }
        catch (HttpOperationException ex) when (ex.StatusCode == HttpStatusCode.BadRequest)
        {
            questionAnswerPairs = new List<QuestionAnswerPair>();

            _logger.LogWarning(
                "No questions could be generated for the given content. " +
                "Possibly due to a content filter being triggered.");
        }

        foreach (var item in questionAnswerPairs)
        {
            yield return item;
        }
    }

    protected abstract KernelFunction GetPromptTemplate(Kernel kernel);
}
