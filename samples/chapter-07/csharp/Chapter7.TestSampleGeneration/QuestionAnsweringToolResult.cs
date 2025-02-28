namespace Chapter7.TestSampleGeneration;

public class QuestionAnsweringToolResult
{
    public required string Response { get; set; }
    public required List<TextUnit> Context { get; set; }
}