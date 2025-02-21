using System.Text.Json;
using Chapter7.ContentPreprocessing.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Microsoft.SemanticKernel.Embeddings;
using Qdrant.Client;

namespace Chapter7.ContentPreprocessing.ProcessingSteps;

public class ImportChunksProcessStep
{
    private Kernel _kernel;
    private ILogger _logger;
    private IVectorStore _vectorStore;

    public ImportChunksProcessStep(IConfiguration configuration, ILogger logger)
    {
        var vectorStoreConfiguration = configuration
            .GetSection("VectorStore").Get<VectorStoreConfiguration>();
        
        _kernel = KernelFactory.CreateKernel(configuration);
        _logger = logger;

        _vectorStore = new QdrantVectorStore(new QdrantClient(vectorStoreConfiguration!.HostName));
    }

    public async Task ProcessAsync(string baseDirectory)
    {
        var embeddingService = _kernel.GetRequiredService<ITextEmbeddingGenerationService>();
        var collection = _vectorStore.GetCollection<ulong, TextUnit>("content");

        await collection.CreateCollectionIfNotExistsAsync();
        
        var chunksDirectory = Path.Join(baseDirectory, "chunks");
        var chunkFiles = Directory.GetFiles(chunksDirectory, "*.json");

        ulong generatedId = 0L;
        
        foreach (var chunkFile in chunkFiles)
        {
            await using var inputStream = File.OpenRead(chunkFile);
            var textChunk = await JsonSerializer.DeserializeAsync<TextChunk>(inputStream);

            if (textChunk == null)
            {
                _logger.LogWarning("Fragment {ChunkFile} could not be processed.", chunkFile);
                continue;
            }

            var embeddingVector = await embeddingService.GenerateEmbeddingAsync(textChunk.Content);

            var textUnit = new TextUnit
            {
                Id = generatedId++,
                Content = textChunk.Content,
                ChunkId = textChunk.Id,
                OriginalFileName = textChunk.OriginalFileName,
                Embedding = embeddingVector
            };

            await collection.UpsertAsync(textUnit);
            
            _logger.LogInformation("Imported chunk {ChunkFile}.", chunkFile);
        }
    }
}