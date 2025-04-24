using Chapter10.ComplexProcess.Data;

namespace Chapter10.SimpleProcess.Steps;

public class FinalizeArticleStep : KernelProcessStep
{
    [KernelFunction]
    public async Task FinalizeArticle(FinalizeArticleInput input)
    {
        await using var fileStream = File.OpenWrite("output.md");
        await using var outputWriter = new StreamWriter(fileStream);

        await outputWriter.WriteLineAsync($"# {input.Title}");
        await outputWriter.WriteLineAsync();

        foreach (var section in input.Sections)
        {
            await outputWriter.WriteLineAsync($"## {section.Title}");
            await outputWriter.WriteLineAsync();
            await outputWriter.WriteLineAsync(section.Content);
            await outputWriter.WriteLineAsync();
        }

        outputWriter.Close();
    }
}