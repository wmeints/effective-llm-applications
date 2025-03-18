using Chapter9.PromptChainContentGeneration.Steps;
using Microsoft.Extensions.Configuration;

namespace Chapter9.PromptChainContentGeneration.Tests.Steps;

public class ResearchContentStepTests
{
    private readonly IConfiguration _configuration;
    private readonly ResearchContentStep _researchContentStep;

    public ResearchContentStepTests()
    {
        _configuration = TestObjectFactory.GetTestConfiguration();
        _researchContentStep = new ResearchContentStep(_configuration);
    }

    [Fact]
    public async Task InvokeAsync_ValidTopic_ReturnsResearchContentResult()
    {
        // Arrange
        var topic = "Artificial Intelligence";

        // Act
        var result = await _researchContentStep.InvokeAsync(topic);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.SearchResults);
        Assert.NotEmpty(result.SearchResults);
    }

    [Theory]
    [InlineData("Machine Learning")]
    [InlineData("Cloud Computing")]
    public async Task InvokeAsync_DifferentTopics_ReturnsResults(string topic)
    {
        // Arrange
        // Topic is provided as parameter

        // Act
        var result = await _researchContentStep.InvokeAsync(topic);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.SearchResults);
        Assert.NotEmpty(result.SearchResults);
    }

    [Fact]
    public async Task InvokeAsync_LongTopic_ReturnsResult()
    {
        // Arrange
        var longTopic = "The impact of artificial intelligence on society, healthcare, education, transportation, and the future of work in the next decade";

        // Act
        var result = await _researchContentStep.InvokeAsync(longTopic);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.SearchResults);
        Assert.NotEmpty(result.SearchResults);
    }
}