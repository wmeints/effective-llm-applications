using Chapter7.RetrievalAugmentedGeneration.Models;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;

namespace Chapter7.RetrievalAugmentedGeneration.Services;

public class QuestionAnsweringTool(
    Kernel kernel, VectorStore vectorStore,
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator)
{
    public async Task<QuestionAnsweringToolResult> AnswerAsync(string question)
    {
        var promptTemplateContent = File.ReadAllText("Prompts/answer-question.yaml");

        var promptTemplate = kernel.CreateFunctionFromPromptYaml(
            promptTemplateContent, new HandlebarsPromptTemplateFactory()
            {
                AllowDangerouslySetContent = true
            });

        var collection = vectorStore.GetCollection<ulong, TextUnit>("content");

        var questionEmbedding = await embeddingGenerator.GenerateAsync(question);

        var searchResponse = collection.SearchAsync(questionEmbedding, 3);

        var fragments = new List<TextUnit>();

        await foreach (var fragment in searchResponse)
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