using Chapter7.ContentPreprocessing.ProcessingSteps;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
});

var logger = loggerFactory.CreateLogger<Program>();

var chunkingProcessStep = new ChunkingProcessStep(logger);

var createValidationDatasetProcessStep =
    new CreateValidationDatasetProcessStep(configuration, logger);

await chunkingProcessStep.ProcessAsync("Content");
await createValidationDatasetProcessStep.ProcessAsync("Content");
