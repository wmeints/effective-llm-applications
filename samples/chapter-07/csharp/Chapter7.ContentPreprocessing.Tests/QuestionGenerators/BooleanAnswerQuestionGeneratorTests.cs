using Chapter7.ContentPreprocessing.QuestionGenerators;
using Chapter7.ContentPreprocessing.Tests.TestResources;
using Shouldly;

namespace Chapter7.ContentPreprocessing.Tests.QuestionGenerators;

public class BooleanAnswerQuestionGeneratorTests
{
    [Fact]
    [Trait("Category", "Unstable")]
    public async Task GenerateQuestionsAsync_ReturnsTrueFalsePairs()
    {
        var kernel = TestObjectFactory.CreateKernel();
        var generator = new BooleanAnswerQuestionGenerator(kernel);
        var content = TestResource.Read("SampleChunk.txt");

        var result = await generator.GenerateQuestionsAsync(content, 4).ToListAsync();

        foreach (var pair in result)
        {
            pair.Answer.ToLower().ShouldBeOneOf("true", "false");
        }
    }
    
    [Fact]
    public async Task GenerateQuestionsAsync_ReturnsCorrectNumberOfPairs()
    {
        var kernel = TestObjectFactory.CreateKernel();
        var generator = new BooleanAnswerQuestionGenerator(kernel);
        var content = TestResource.Read("SampleChunk.txt");

        var result = await generator.GenerateQuestionsAsync(content, 2).ToListAsync();

        result.Count().ShouldBe(2);
    }
}