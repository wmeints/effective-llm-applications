using System.Globalization;
using System.Net;
using System.Text.Json;
using Chapter7.TestSampleGeneration;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Microsoft.SemanticKernel.Embeddings;
using Polly;
using Qdrant.Client;

// STEP 0: Initialize the application components
//
// This step initializes all the components we need to run the sample.
// - The kernel configuration for generating content
// - The logger to track progress of the sample
// - The vector store to store the content of the RAG pipeline
// - The question answering tool that implements the RAG pipeline
// ------------------------------------------------------------

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
});

var logger = loggerFactory.CreateLogger<Program>();

var kernel = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(
        deploymentName: configuration["LanguageModel:CompletionModel"]!,
        endpoint: configuration["LanguageModel:Endpoint"]!,
        apiKey: configuration["LanguageModel:ApiKey"]!
    )
    .AddAzureOpenAIEmbeddingGenerator(
        deploymentName: configuration["LanguageModel:EmbeddingModel"]!,
        endpoint: configuration["LanguageModel:Endpoint"]!,
        apiKey: configuration["LanguageModel:ApiKey"]!
    )
    .Build();

var vectorStore = new QdrantVectorStore(new QdrantClient("localhost"), true);
var embeddingGenerator = kernel.GetRequiredService<IEmbeddingGenerator<string, Embedding<float>>>();

var answeringTool = new QuestionAnsweringTool(
    kernel,
    vectorStore,
    embeddingGenerator);

// STEP 1: Index the content
// ------------------------------------------------------------

var contentIndexer = new ContentIndexer(
    vectorStore,
    embeddingGenerator,
    loggerFactory.CreateLogger<ContentIndexer>());

await contentIndexer.ProcessContentAsync();

// STEP 2: Generate the test samples
//
// This step generates the test samples based on the synthetic questions we generated
// in the Chapter7.ValidationDataGeneration project. The questions are stored in a CSV file.
// ------------------------------------------------------------

using var reader = new StreamReader(File.OpenRead("Input/validation-data.csv"));
using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

var validationQuestionRecords = csv.GetRecords<ValidationDataRecord>().ToList();
var testSampleRecords = new List<TestSampleRecord>();

logger.LogInformation("Processing {Count} questions", validationQuestionRecords.Count);

// NOTE: We're limiting the number of questions to 25, otherwise this sample gets too big to run in a reasonable time.

foreach (var record in validationQuestionRecords.SelectRandom(25))
{
    logger.LogInformation("Processing question {Question}", record.Question);

    var result = await answeringTool.AnswerAsync(record.Question);

    logger.LogInformation("Result: {Result}", result.Response);

    var testSampleRecord = new TestSampleRecord(
        record.Question, result.Response, result.Context);

    testSampleRecords.Add(testSampleRecord);
}

// STEP 3: Write the test samples to a CSV file
//
// This step saves the test samples to a JSON file, which we use with the python script
// to calculate the faithfulness of the RAG pipeline. We're using JSON because it's
// easier to serialize a complex structure than a CSV file.
// ------------------------------------------------------------

using var outputStream = File.OpenWrite("test-samples.json");

await JsonSerializer.SerializeAsync(
    outputStream, testSampleRecords,
    new JsonSerializerOptions
    {
        WriteIndented = true
    });
