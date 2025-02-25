using Chapter7.RetrievalAugmentedGeneration.Models;

namespace Chapter7.RetrievalAugmentedGeneration.Services;

public class QuestionAnsweringToolResult
{
    public required string Response { get; set; }
    public required List<TextUnit> Context { get; set; }
}