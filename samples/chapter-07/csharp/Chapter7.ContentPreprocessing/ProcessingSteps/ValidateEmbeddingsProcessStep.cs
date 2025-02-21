using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;
using Chapter7.ContentPreprocessing.Shared;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Microsoft.SemanticKernel.Embeddings;

namespace Chapter7.ContentPreprocessing.ProcessingSteps;

public class ValidateEmbeddingsProcessStep
{
    private readonly Kernel _kernel;
    private readonly IVectorStore _vectorStore;
    private readonly ILogger _logger;

    public ValidateEmbeddingsProcessStep(IConfiguration configuration, ILogger logger)
    {
        _kernel = KernelFactory.CreateKernel(configuration);
        _vectorStore = VectorStoreFactory.CreateVectorStore(configuration);
        _logger = logger;

    }

    public async Task ProcessAsync(string baseDirectory)
    {
        var numberOfResults = 10;

        var contentCollection = _vectorStore.GetCollection<ulong, TextUnit>("content");
        var validationRecords = LoadValidationDatasetAsync(baseDirectory);
        var textEmbeddingService = _kernel.GetRequiredService<ITextEmbeddingGenerationService>();

        var results = new List<double>();

        foreach (var validationDataRecord in validationRecords)
        {
            var queryVector = await textEmbeddingService.GenerateEmbeddingAsync(validationDataRecord.Question);

            var searchResults = await contentCollection.VectorizedSearchAsync(queryVector, new VectorSearchOptions
            {
                Top = numberOfResults
            });

            var score = await CalculateNDCG(searchResults, validationDataRecord.FragmentId);

            results.Add(score);
        }

        var meanScore = results.Count > 0 ? results.Average() : 0.0;

        _logger.LogInformation("Mean NDCG Score: {MeanScore} with K = {K}", meanScore, numberOfResults);
    }

    private async Task<double> CalculateNDCG(VectorSearchResults<TextUnit> searchResults, string expectedId)
    {
        var items = await searchResults.Results.ToListAsync();
        int index = items.FindIndex(result => result.Record.ChunkId == expectedId);

        // If the expected result is not found, return 0.
        if (index < 0)
        {
            return 0.0;
        }

        // Compute Discounted Cumulative Gain (DCG) for a single relevant item.
        // We use the formula: DCG = 1 / log2(rank + 1)
        // Since index is 0-based, add 2 to the index to obtain the proper rank.
        double dcg = 1.0 / Math.Log(index + 2, 2);

        // For one relevant item, the ideal DCG is 1 (if it were at the top).
        // Thus, normalized DCG equals the computed DCG.
        return dcg;
    }

    private List<ValidationDataRecord> LoadValidationDatasetAsync(string baseDirectory)
    {
        using var inputReader = new StreamReader(File.OpenRead(Path.Join(baseDirectory, "validation-data.csv")));
        using var csv = new CsvReader(inputReader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            NewLine = Environment.NewLine
        });

        return csv.GetRecords<ValidationDataRecord>().ToList();
    }
}