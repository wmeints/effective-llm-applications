using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Chapter7.ContentPreprocessing.ProcessingSteps;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.Text;

using Microsoft.Extensions.Logging.Console;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
});

var logger = loggerFactory.CreateLogger<Program>();

var chunkingProcessStep = new ChunkingProcessStep(logger);
var createValidationDatasetProcessStep = new CreateValidationDatasetProcessStep(configuration, logger);
var importChunksProcessStep = new ImportChunksProcessStep(configuration, logger);

// await chunkingProcessStep.ProcessAsync("Content");
// await createValidationDatasetProcessStep.ProcessAsync("Content");

await importChunksProcessStep.ProcessAsync("Content");