using System.Diagnostics.SymbolStore;
using Chapter7.RetrievalAugmentedGeneration.Models;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Text;
using OpenAI.Chat;

namespace Chapter7.RetrievalAugmentedGeneration.Services;

public class ContentIndexer(
    VectorStore vectorStore,
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
    ILogger<ContentIndexer> logger)
{
    public async Task ProcessContentAsync()
    {
        logger.LogInformation("Processing content");

        ulong currentIdentifier = 1L;
        
        var files = Directory.GetFiles("Content", "*.md",
            SearchOption.AllDirectories);

        var textUnits = new List<TextUnit>();

        var collection = vectorStore.GetCollection<ulong, TextUnit>("content");

        await collection.EnsureCollectionExistsAsync();

        foreach (var file in files)
        {
            logger.LogInformation("Processing {OriginalFileName}", file);

            var lines = await File.ReadAllLinesAsync(file);

            var chunks = TextChunker.SplitMarkdownParagraphs(
                lines, maxTokensPerParagraph: 1000);

            logger.LogInformation("Found {Count} chunks", chunks.Count);

            foreach (var chunk in chunks)
            {
                var embedding = await embeddingGenerator.GenerateAsync(chunk);

                var textUnit = new TextUnit
                {
                    Content = chunk,
                    Embedding = embedding.Vector,
                    OriginalFileName = file,
                    Id = currentIdentifier++
                };

                await collection.UpsertAsync(textUnit);
            }

            logger.LogInformation(
                "Processed {OriginalFileName} with {ChunkCount} chunks",
                file, chunks.Count);
        }
    }
}