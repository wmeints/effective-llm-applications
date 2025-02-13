using Chapter7.ChatWithRetrieval.Models;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Data;
using Microsoft.SemanticKernel.Embeddings;

namespace Chapter7.ChatWithRetrieval.Services;

public class CompletionService(
    Kernel kernel, IVectorStore vectorStore,
    ITextEmbeddingGenerationService embeddingGenerator,
    IChatCompletionService chatCompletions)
{
    public async Task<string> GenerateResponse(string prompt)
    {
        var textCollection = vectorStore.GetCollection<ulong, TextUnit>("content");

        var textSearch = new VectorStoreTextSearch<TextUnit>(
            textCollection,
            embeddingGenerator,
            new TextUnitStringMapper(),
            new TextUnitTextSearchResultMapper());

        var searchFunction = textSearch.CreateGetTextSearchResults();

        kernel.Plugins.AddFromFunctions("SearchPlugin", [searchFunction]);

        var chatHistory = new ChatHistory();
        chatHistory.AddSystemMessage("You're a friendly assistant. Your name is Ricardo");
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