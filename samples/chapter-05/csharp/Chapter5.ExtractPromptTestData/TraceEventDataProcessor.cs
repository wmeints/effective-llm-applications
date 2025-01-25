using System.Text.Json;

public class TraceEventDataProcessor
{
    private string? _currentPrompt;
    private bool _isParsingPair;

    public List<PromptCompletionPair> ParsedPromptCompletionPairs { get; } = new();

    public void ProcessEvent(TraceEventData eventData)
    {
        if (eventData.Message == "gen_ai.content.prompt")
        {
            _currentPrompt = eventData.Properties.Prompt;
            _isParsingPair = true;
        }

        if (_isParsingPair)
        {
            if (eventData.Message == "gen_ai.content.completion")
            {
                if (!string.IsNullOrEmpty(_currentPrompt))
                {
                    ParsedPromptCompletionPairs.Add(new PromptCompletionPair()
                    {
                        Prompt = _currentPrompt,
                        Completion = eventData.Properties.Completion!
                    });
                }
            }
        }
    }
}