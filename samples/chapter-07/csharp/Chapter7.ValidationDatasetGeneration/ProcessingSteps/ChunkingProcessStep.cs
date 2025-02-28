using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Chapter7.ValidationDatasetGeneration.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.Text;

namespace Chapter7.ContentPreprocessing.ProcessingSteps;

public class ChunkingProcessStep
{
    private readonly ILogger _logger;

    public ChunkingProcessStep(ILogger logger)
    {
        _logger = logger;
    }

    public async Task ProcessAsync(string baseDirectory)
    {
        var inputDirectory = Path.Join(baseDirectory, "input");
        var chunkDirectory = Path.Join(baseDirectory, "chunks");

        if (!Directory.Exists(inputDirectory))
        {
            throw new InvalidOperationException("Input directory does not exist");
        }

        if (Directory.Exists(chunkDirectory))
        {
            Directory.Delete(chunkDirectory, recursive: true);
        }

        Directory.CreateDirectory(chunkDirectory);

        var sourceFiles = Directory.GetFiles(inputDirectory, "*.md", SearchOption.AllDirectories);

        foreach (var filePath in sourceFiles)
        {
            _logger.LogInformation("Chunking {FilePath}", filePath);

            var sourceLines = await File.ReadAllLinesAsync(filePath);

            var chunks = TextChunker.SplitMarkdownParagraphs(sourceLines,
                maxTokensPerParagraph: 500, overlapTokens: 0);

            var textUnits = chunks.Select(chunkContent => new TextChunk(
                GenerateHash(chunkContent), chunkContent,
                Path.GetRelativePath(inputDirectory, filePath))).ToList();

            foreach (var textUnit in textUnits)
            {
                await using var outputStream = File.OpenWrite(Path.Join(chunkDirectory, $"{textUnit.Id}.json"));
                await JsonSerializer.SerializeAsync(outputStream, textUnit);
            }

            _logger.LogInformation("Chunked {FilePath} into {ChunkCount} chunks", filePath, textUnits.Count);
        }
    }

    private string GenerateHash(string content)
    {
        using var hashingAlgorithm = SHA1.Create();
        var hash = hashingAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(content));

        return Convert.ToHexStringLower(hash);
    }
}