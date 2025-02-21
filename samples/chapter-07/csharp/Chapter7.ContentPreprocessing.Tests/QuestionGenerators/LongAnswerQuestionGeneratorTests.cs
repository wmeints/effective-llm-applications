using Chapter7.ContentPreprocessing.QuestionGenerators;
using Chapter7.ContentPreprocessing.Tests.TestResources;
using Shouldly;

namespace Chapter7.ContentPreprocessing.Tests.QuestionGenerators;

public class LongAnswerQuestionGeneratorTests
{
    [Fact]
    [Trait("Category", "Unstable")]
    public async Task GenerateQuestions_ReturnsDetailedQnaPairs()
    {
        var kernel = TestObjectFactory.CreateKernel();
        var generator = new DetailedQuestionGenerator(kernel);
        var content = TestResource.Read("SampleChunk.txt");
        
        var result = await generator.GenerateQuestionsAsync(content, 4).ToListAsync();
        
        foreach (var pair in result)
        {
            pair.Question.Split(" ").Length.ShouldBeGreaterThanOrEqualTo(10, "Question should be more than 10 words");
            pair.Answer.Split(".").Length.ShouldBeGreaterThan(1, "Answer should be more than one sentence");
            pair.Answer.Split(" ").Length.ShouldBeGreaterThanOrEqualTo(10, "Answer should be more than 10 words");
        }
    }
    
    [Fact]
    public async Task GenerateQuestionsAsync_ReturnsCorrectNumberOfPairs()
    {
        var kernel = TestObjectFactory.CreateKernel();
        var generator = new DetailedQuestionGenerator(kernel);
        var content = TestResource.Read("SampleChunk.txt");

        var result = await generator.GenerateQuestionsAsync(content, 2).ToListAsync();

        result.Count().ShouldBe(2);
    }
}