using Chapter7.ChatWithRetrieval.Models;
using Microsoft.SemanticKernel.Data;

namespace Chapter7.ChatWithRetrieval.Services;

public class TextUnitTextSearchResultMapper : ITextSearchResultMapper
{
    public TextSearchResult MapFromResultToTextSearchResult(object result)
    {
        if (result is TextUnit textUnit)
        {
            return new TextSearchResult(value: textUnit.Content)
            {
                Link = textUnit.OriginalFileName,
                Name = textUnit.Id.ToString()
            };
        }

        throw new ArgumentException("Invalid result object");
    }
}