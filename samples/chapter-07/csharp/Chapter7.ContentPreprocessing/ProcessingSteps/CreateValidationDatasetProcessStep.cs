using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Chapter7.ContentPreprocessing.QuestionGenerators;
using Chapter7.ContentPreprocessing.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Chapter7.ContentPreprocessing.ProcessingSteps;

public class CreateValidationDatasetProcessStep
{
    private readonly ILogger _logger;
    private readonly List<IQuestionGenerator> _questionGenerators;

    public CreateValidationDatasetProcessStep(IConfiguration configuration, ILogger logger)
    {
        _logger = logger;
        
        var kernel = KernelFactory.CreateKernel(configuration);

        _questionGenerators =
        [
            new ShortAnswerQuestionGenerator(kernel),
            new DetailedQuestionGenerator(kernel),
            new BooleanAnswerQuestionGenerator(kernel)
        ];
    }

    public async Task ProcessAsync(string baseDirectory)
    {
        var chunksDirectory = Path.Join(baseDirectory, "chunks");
        var chunkFiles = Directory.GetFiles(chunksDirectory, "*.json");
        var generatedValidationRecords = new List<ValidationDataRecord>();

        foreach (var chunkFile in chunkFiles)
        {
            await using var inputStream = File.OpenRead(chunkFile);
            var textUnit = await JsonSerializer.DeserializeAsync<TextChunk>(inputStream);

            if (textUnit == null)
            {
                _logger.LogWarning("Fragment {ChunkFile} could not be processed.", chunkFile);
                continue;
            }
            
            var questionAnswerPairs = await GenerateQuestionAnswerPairs(textUnit).ToListAsync();
            
            _logger.LogInformation("Done generating samples for chunk {ChunkFile}.", chunkFile);
            
            var validationRecords = questionAnswerPairs.Select(x => new ValidationDataRecord
            {
                Id = GenerateHash(textUnit.Content),
                OriginalFileName = textUnit.OriginalFileName,
                FragmentId = textUnit.Id,
                Answer = x.Answer,
                Question = x.Question
            });
            
            generatedValidationRecords.AddRange(validationRecords);
        }

        await using var outputStream = File.OpenWrite(Path.Join(baseDirectory, "validation-dataset.json"));
        await JsonSerializer.SerializeAsync(outputStream, generatedValidationRecords);
    }

    private async IAsyncEnumerable<QuestionAnswerPair> GenerateQuestionAnswerPairs(TextChunk textChunk)
    {
        foreach (var questionGenerator in _questionGenerators)
        {
            _logger.LogInformation("Generating questions using the {GeneratorType}", questionGenerator.GetType());

            await foreach (var question in questionGenerator.GenerateQuestionsAsync(textChunk.Content, 4))
            {
                yield return question;
            }
        }
    }

    private string GenerateHash(string content)
    {
        using var hashingAlgorithm = SHA1.Create();
        var hash = hashingAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(content));

        return Convert.ToHexStringLower(hash);
    }
}