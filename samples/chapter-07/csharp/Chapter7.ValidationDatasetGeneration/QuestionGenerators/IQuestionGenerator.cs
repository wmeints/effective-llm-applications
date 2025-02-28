using Chapter7.ContentPreprocessing.QuestionGenerators;

namespace Chapter7.ValidationDatasetGeneration.QuestionGenerators;

public interface IQuestionGenerator
{
    IAsyncEnumerable<QuestionAnswerPair> GenerateQuestionsAsync(string content, int numberOfQuestions);
}