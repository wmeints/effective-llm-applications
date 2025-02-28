using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;

namespace Chapter7.TestSampleGeneration;

public class QuestionAnsweringTool(
    Kernel kernel, IVectorStore vectorStore,
    ITextEmbeddingGenerationService embeddingGenerator)
{
    public async Task<QuestionAnsweringToolResult> AnswerAsync(string question)
    {
        var promptTemplateContent = File.ReadAllText("Prompts/answer-question.yaml");

        var promptTemplate = kernel.CreateFunctionFromPromptYaml(
            promptTemplateContent, new HandlebarsPromptTemplateFactory());

        var collection = vectorStore.GetCollection<ulong, TextUnit>("content");

        var questionEmbedding = await embeddingGenerator.GenerateEmbeddingAsync(
            question);

        var searchOptions = new VectorSearchOptions
        {
            Top = 3,
        };

        var searchResponse = await collection.VectorizedSearchAsync(
            questionEmbedding, searchOptions);

        var fragments = new List<TextUnit>();

        await foreach (var fragment in searchResponse.Results)
        {
            fragments.Add(fragment.Record);
        }

        var response = await promptTemplate.InvokeAsync(kernel, new KernelArguments
        {
            ["question"] = question,
            ["fragments"] = fragments
        });

        return new QuestionAnsweringToolResult
        {
            Response = response.GetValue<string>()!,
            Context = fragments
        };
    }
}