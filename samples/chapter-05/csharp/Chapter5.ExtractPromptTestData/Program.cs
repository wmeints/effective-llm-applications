using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var blobServiceClient = new BlobServiceClient(configuration["ConnectionStrings:BlobStorage"]);
var traceContainerClient = blobServiceClient.GetBlobContainerClient("am-apptraces");
var processor = new TraceEventDataProcessor();

await foreach (var blobItem in traceContainerClient.GetBlobsAsync())
{
    if (blobItem.Name.EndsWith(".json"))
    {
        var blobClient = traceContainerClient.GetBlobClient(blobItem.Name);
        using var reader = new StreamReader(blobClient.OpenRead());

        while (!reader.EndOfStream)
        {
            var rawEventData = reader.ReadLine();

            var eventData = JsonSerializer.Deserialize<TraceEventData>(rawEventData!, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement
            });

            if (eventData!.Message == "gen_ai.content.completion" || eventData.Message == "gen_ai.content.prompt")
            {
                processor.ProcessEvent(eventData);
            }
        }
    }
}

await using var outputStream = File.CreateText("output.csv");

outputStream.WriteLine("Prompt;Completion");

foreach (var pair in processor.ParsedPromptCompletionPairs)
{
    outputStream.WriteLine("{0};{1}", pair.Prompt,pair.Completion);
}
