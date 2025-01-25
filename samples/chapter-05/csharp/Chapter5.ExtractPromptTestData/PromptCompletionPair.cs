public class PromptCompletionPair
{
    public string Prompt { get; set; }
    public string Completion { get; set; }

    public override string ToString()
    {
        return $"{Prompt}\n{Completion}\n";
    }
}