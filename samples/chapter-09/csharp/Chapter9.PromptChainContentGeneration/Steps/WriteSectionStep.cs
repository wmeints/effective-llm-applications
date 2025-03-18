namespace Chapter9.PromptChainContentGeneration.Steps;

public class WriteSectionStep
{
    private readonly KernelFunction _writeSectionPromptTemplate;
    private readonly Kernel _kernel;

    public WriteSectionStep(Kernel kernel)
    {
        _kernel = kernel;
        _writeSectionPromptTemplate = kernel.CreateFunctionFromPromptYaml(
            EmbeddedResource.Read("Prompts.write-section.yml"),
            new HandlebarsPromptTemplateFactory());
    }

    public async Task<GenerateSectionContentResult> InvokeAsync(
        string topic, string sectionTitle, string query, string searchResults)
    {
        var promptExecutionSettings = new OpenAIPromptExecutionSettings
        {
            ResponseFormat = typeof(GenerateSectionContentResult)
        };

        var response = await _writeSectionPromptTemplate.InvokeAsync(
            _kernel, new KernelArguments(promptExecutionSettings)
            {
                ["topic"] = topic,
                ["sectionTitle"] = sectionTitle,
                ["query"] = query,
                ["searchResults"] = searchResults
            });

        var responseData = response.GetValue<string>()!;
        var result = JsonSerializer.Deserialize<GenerateSectionContentResult>(responseData);

        if (result is null)
        {
            throw new InvalidOperationException("Failed to generate section content");
        }

        return result;
    }
}


public record GenerateSectionContentResult(string Title, string Content);
