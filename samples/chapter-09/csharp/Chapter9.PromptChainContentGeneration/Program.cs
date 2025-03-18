using System.Text;
using System.Text.Json;
using Chapter9.PromptChainContentGeneration.Steps;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var kernel = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(
        configuration["LanguageModel:DeploymentName"]!,
        configuration["LanguageModel:Endpoint"]!,
        configuration["LanguageModel:ApiKey"]!
    ).Build();

var researchContentStep = new ResearchContentStep(configuration);
var createOutlineStep = new CreateOutlineStep(kernel);
var researchSectionStep = new ResearchSectionStep(kernel, configuration);
var writeSectionStep = new WriteSectionStep(kernel);

var topic = "The importance of securing your AI agents in production";

var researchedContent = await researchContentStep.InvokeAsync(topic);
var outline = await createOutlineStep.InvokeAsync(topic, researchedContent.SearchResults);

var outputBuilder = new StringBuilder();

outputBuilder.AppendLine($"# {outline.Title}");

foreach (var section in outline.Sections)
{
    var researchedSectionContent = await researchSectionStep.InvokeAsync(topic, section);
    var sectionContent = await writeSectionStep.InvokeAsync(
        topic, section, researchedSectionContent.Query,
        researchedSectionContent.SearchResults);

    outputBuilder.AppendLine();
    outputBuilder.AppendLine($"## {sectionContent.Title}");
    outputBuilder.AppendLine(sectionContent.Content);
}

Console.WriteLine(outputBuilder.ToString());