using System.Text;
using Chapter7.ChatWithRetrieval.Models;
using Microsoft.SemanticKernel.Data;

namespace Chapter7.ChatWithRetrieval.Services;

public class CitationsTextUnitStringMapper : ITextSearchStringMapper
{
    public string MapFromResultToString(object result)
    {
        if (result is TextUnit textUnit)
        {
            var outputBuilder = new StringBuilder();

            outputBuilder.AppendLine($"Name: {textUnit.Id}");
            outputBuilder.AppendLine($"Value: {textUnit.Content}");
            outputBuilder.AppendLine($"Link: {textUnit.OriginalFileName}");

            return outputBuilder.ToString();
        }

        throw new ArgumentException("Invalid result object");
    }
}