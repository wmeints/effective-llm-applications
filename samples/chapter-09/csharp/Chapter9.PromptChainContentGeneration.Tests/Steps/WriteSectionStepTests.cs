using System;
using System.Threading.Tasks;
using Chapter9.PromptChainContentGeneration.Steps;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Xunit;

namespace Chapter9.PromptChainContentGeneration.Tests.Steps
{
    public class WriteSectionStepTests
    {
        private readonly WriteSectionStep _writeSectionStep;
        private readonly Kernel _kernel;
        private readonly IConfiguration _configuration;

        public WriteSectionStepTests()
        {
            _configuration = TestObjectFactory.GetTestConfiguration();
            _kernel = TestObjectFactory.GetKernel(_configuration);
            _writeSectionStep = new WriteSectionStep(_kernel);
        }

        [Fact]
        public async Task InvokeAsync_WithValidInputs_ReturnsGeneratedContent()
        {
            // Arrange
            var topic = "AI Ethics";
            var sectionTitle = "Privacy Concerns";
            var query = "What are the main privacy concerns in AI?";
            var searchResults = @"
                AI systems often collect vast amounts of personal data.
                Privacy concerns include unauthorized data collection, lack of consent,
                and potential for surveillance.
                Studies show 78% of users are concerned about AI privacy implications.
            ";

            // Act
            var result = await _writeSectionStep.InvokeAsync(topic, sectionTitle, query, searchResults);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Title);
            Assert.NotEmpty(result.Title);
            Assert.NotNull(result.Content);
            Assert.NotEmpty(result.Content);

            // Verify content is relevant to the topic and search results
            Assert.Contains("privacy", result.Content.ToLower());
            Assert.Contains("ai", result.Content.ToLower());
        }

        [Fact]
        public async Task InvokeAsync_WithMinimalInputs_StillGeneratesContent()
        {
            // Arrange
            var topic = "Climate Change";
            var sectionTitle = "Impact on Ecosystems";
            var query = "How does climate change affect ecosystems?";
            var searchResults = "Limited search results available.";

            // Act
            var result = await _writeSectionStep.InvokeAsync(topic, sectionTitle, query, searchResults);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Title);
            Assert.NotEmpty(result.Title);
            Assert.NotNull(result.Content);
            Assert.NotEmpty(result.Content);
        }

        [Fact]
        public async Task InvokeAsync_WithComplexTopic_ReturnsStructuredContent()
        {
            // Arrange
            var topic = "Quantum Computing";
            var sectionTitle = "Quantum Entanglement";
            var query = "Explain quantum entanglement and its significance";
            var searchResults = @"
                Quantum entanglement occurs when pairs of particles interact in ways such that
                the quantum state of each particle cannot be described independently.
                Einstein called it 'spooky action at a distance.'
                It's fundamental to quantum computing and quantum information processing.
                Recent experiments have demonstrated entanglement over distances exceeding 1,200 kilometers.
            ";

            // Act
            var result = await _writeSectionStep.InvokeAsync(topic, sectionTitle, query, searchResults);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Title);
            Assert.NotEmpty(result.Title);
            Assert.NotNull(result.Content);
            Assert.NotEmpty(result.Content);

            // Verify content contains key concepts from the search results
            Assert.Contains("entanglement", result.Content.ToLower());
            Assert.Contains("quantum", result.Content.ToLower());
        }

        [Fact]
        public async Task InvokeAsync_WithEmptyTopic_ThrowsArgumentException()
        {
            // Arrange
            var topic = "";
            var sectionTitle = "Some Section";
            var query = "Some query";
            var searchResults = "Some results";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _writeSectionStep.InvokeAsync(topic, sectionTitle, query, searchResults));
        }
    }
}