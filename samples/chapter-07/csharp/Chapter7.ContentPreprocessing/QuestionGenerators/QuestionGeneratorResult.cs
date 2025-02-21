using Chapter7.ContentPreprocessing.ProcessingSteps;

namespace Chapter7.ContentPreprocessing.QuestionGenerators;

public class QuestionGeneratorResult
{
    public List<QuestionAnswerPair> QuestionAnswerPairs { get; set; } = new();
}