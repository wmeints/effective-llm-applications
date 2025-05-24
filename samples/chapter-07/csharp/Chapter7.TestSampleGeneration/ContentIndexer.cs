using System.Net;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Text;
using OpenAI.Chat;
using Polly;
using Polly.Retry;

namespace Chapter7.TestSampleGeneration;

public class ContentIndexer
{
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly ILogger _logger;
    private readonly VectorStore _vectorStore;
    private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;

    public ContentIndexer(VectorStore vectorStore, IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator, ILogger<ContentIndexer> logger)
    {
        _logger = logger;
        _vectorStore = vectorStore;
        _embeddingGenerator = embeddingGenerator;

        _retryPolicy = Policy
            .Handle<HttpOperationException>(ex => ex.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(
                5, // number of retries
                attempt => TimeSpan.FromSeconds(Math.Min(30, Math.Pow(2, attempt) * 5)), // exponential backoff starting at 5s
                (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning(
                        "Request rate limited. Waiting {delay} seconds before retry. Attempt {retryCount} of 5.",
                        timeSpan.TotalSeconds,
                        retryCount);
                }
            );
    }


    public async Task ProcessContentAsync()
    {
        _logger.LogInformation("Processing content");

        ulong currentIdentifier = 1L;
        var files = Directory.GetFiles("Content", "*.md", SearchOption.AllDirectories);

        var collection = _vectorStore.GetCollection<ulong, TextUnit>("content");

        await collection.EnsureCollectionExistsAsync();

        foreach (var file in files)
        {
            _logger.LogInformation("Processing {OriginalFileName}", file);

            var lines = await File.ReadAllLinesAsync(file);

            var chunks = TextChunker.SplitMarkdownParagraphs(
                lines, maxTokensPerParagraph: 1000);

            _logger.LogInformation("Found {Count} chunks", chunks.Count);

            foreach (var chunk in chunks)
            {
                await _retryPolicy.ExecuteAsync(async () =>
                {
                    var embedding = await _embeddingGenerator.GenerateAsync(chunk);

                    var textUnit = new TextUnit
                    {
                        Content = chunk,
                        Embedding = embedding.Vector,
                        OriginalFileName = file,
                        Id = currentIdentifier++
                    };

                    await collection.UpsertAsync(textUnit);
                });
            }

            _logger.LogInformation(
                "Processed {OriginalFileName} with {ChunkCount} chunks",
                file, chunks.Count);
        }
    }
}