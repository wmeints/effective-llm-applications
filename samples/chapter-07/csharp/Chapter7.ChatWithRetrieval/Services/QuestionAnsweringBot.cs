using Chapter7.ChatWithRetrieval.Models;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Data;
using Microsoft.SemanticKernel.Embeddings;

namespace Chapter7.ChatWithRetrieval.Services;

public class QuestionAnsweringBot(
    Kernel kernel, VectorStore vectorStore,
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
    IChatCompletionService chatCompletions)
{
    public async Task<string> GenerateResponse(string prompt)
    {
        var citationsFilter = new CitationCapturingFilter();
        var textCollection = vectorStore.GetCollection<ulong, TextUnit>("content");

        var textSearch = new VectorStoreTextSearch<TextUnit>(
            textCollection,
            embeddingGenerator,
            new CitationsTextUnitStringMapper(),
            new TextUnitTextSearchResultMapper());

        kernel.Plugins.AddFromObject(textSearch.CreateWithSearch("SearchPlugin"));
        kernel.FunctionInvocationFilters.Add(citationsFilter);

        var chatHistory = new ChatHistory();

        chatHistory.AddSystemMessage(
            "You're a friendly assistant. Your name is " +
            "Ricardo. When answering questions, include " +
            "citations to the relevant information where " +
            "it is referenced in the response.");

        chatHistory.AddUserMessage(prompt);

        var executionSettings = new AzureOpenAIPromptExecutionSettings
        {
            Temperature = 0.6,
            FrequencyPenalty = 0.0,
            PresencePenalty = 0.0,
            MaxTokens = 2500,
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        var response = await chatCompletions.GetChatMessageContentAsync(
            chatHistory, executionSettings, kernel);

        return response.Content!;
    }
}