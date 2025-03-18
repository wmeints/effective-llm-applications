using Chapter9.PromptChainContentGeneration.Steps;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;

namespace Chapter9.PromptChainContentGeneration.Tests.Steps;

public class CreateOutlineStepTests
{
    private readonly Kernel _kernel;
    private readonly CreateOutlineStep _createOutlineStep;

    public CreateOutlineStepTests()
    {
        var config = TestObjectFactory.GetTestConfiguration();
        _kernel = TestObjectFactory.GetKernel(config);

        _createOutlineStep = new CreateOutlineStep(_kernel);
    }

    [Fact]
    public async Task InvokeAsync_ValidTopic_ReturnsCreateOutlineResult()
    {
        // Arrange
        var topic = "Artificial Intelligence";

        // Act
        var result = await _createOutlineStep.InvokeAsync(topic, "");

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Title);
        Assert.NotEmpty(result.Title);
        Assert.NotNull(result.Sections);
        Assert.NotEmpty(result.Sections);
    }

    [Theory]
    [InlineData("")]
    public async Task InvokeAsync_EmptyTopic_ShouldThrowArgumentException(string topic)
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _createOutlineStep.InvokeAsync(topic, ""));
    }

    [Fact]
    public async Task InvokeAsync_LongTopic_ReturnsResult()
    {
        // Arrange
        var longTopic = "The impact of artificial intelligence on society, healthcare, education, transportation, and the future of work in the next decade";

        // Act
        var result = await _createOutlineStep.InvokeAsync(longTopic, "");

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Title);
        Assert.NotEmpty(result.Title);
        Assert.NotNull(result.Sections);
        Assert.NotEmpty(result.Sections);
    }
}