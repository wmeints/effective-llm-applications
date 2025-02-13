using Chapter7.ChatWithRetrieval.Models;
using Microsoft.SemanticKernel.Data;

namespace Chapter7.ChatWithRetrieval.Services;

public class TextUnitStringMapper : ITextSearchStringMapper
{
    public string MapFromResultToString(object result)
    {
        if (result is TextUnit textUnit)
        {
            return textUnit.Content;
        }

        throw new ArgumentException("Invalid result object");
    }
}
