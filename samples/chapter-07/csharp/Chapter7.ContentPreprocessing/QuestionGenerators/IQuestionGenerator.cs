using Chapter7.ContentPreprocessing.ProcessingSteps;

namespace Chapter7.ContentPreprocessing.QuestionGenerators;

/// <summary>
/// Classes implementing this interface are capable of generating question/answer pairs from the given content.
/// </summary>
public interface IQuestionGenerator
{
    /// <summary>
    /// Generate question/answer pairs from the given content.
    /// </summary>
    /// <param name="content">Content to extract question/answer pairs from.</param>
    /// <param name="numberOfQuestions">Number of questions to extract.</param>
    /// <returns>Returns a list of question/answer pairs.</returns>
    IAsyncEnumerable<QuestionAnswerPair> GenerateQuestionsAsync(string content, int numberOfQuestions);
}