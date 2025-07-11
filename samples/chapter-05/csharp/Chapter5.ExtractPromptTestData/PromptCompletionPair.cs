public class PromptCompletionPair
{
    public required string Prompt { get; set; }
    public required string Completion { get; set; }

    public override string ToString()
    {
        return $"{Prompt}\n{Completion}\n";
    }
}