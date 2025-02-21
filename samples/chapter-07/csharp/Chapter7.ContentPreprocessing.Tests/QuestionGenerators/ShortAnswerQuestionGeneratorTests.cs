using Chapter7.ContentPreprocessing.QuestionGenerators;
using Chapter7.ContentPreprocessing.Tests.TestResources;
using Shouldly;

namespace Chapter7.ContentPreprocessing.Tests.QuestionGenerators;

public class ShortAnswerQuestionGeneratorTests
{
    [Fact]
    [Trait("Category", "Unstable")]
    public async Task GenerateQuestions_ReturnsShortQnaPairs()
    {
        var kernel = TestObjectFactory.CreateKernel();
        var generator = new ShortAnswerQuestionGenerator(kernel);
        var content = TestResource.Read("SampleChunk.txt");

        var result = await generator.GenerateQuestionsAsync(content, 4).ToListAsync();

        foreach (var pair in result)
        {
            pair.Question.Split(".").Length.ShouldBe(1, "Question should be one sentence");
            pair.Answer.Split(".").Length.ShouldBeGreaterThanOrEqualTo(1, "Answer should be one sentence");
            pair.Answer.Split(".").Length.ShouldBeLessThanOrEqualTo(2, "Answer should be one sentence");
            pair.Answer.Split(" ").Length.ShouldBeLessThanOrEqualTo(10, "Answer should be less than 10 words");
        }
    }
    
    [Fact]
    public async Task GenerateQuestionsAsync_ReturnsCorrectNumberOfPairs()
    {
        var kernel = TestObjectFactory.CreateKernel();
        var generator = new ShortAnswerQuestionGenerator(kernel);
        var content = TestResource.Read("SampleChunk.txt");

        var result = await generator.GenerateQuestionsAsync(content, 2).ToListAsync();

        result.Count().ShouldBe(2);
    }
}