using System.Text.Json.Serialization;

public class TraceProperties
{
    [JsonPropertyName("gen_ai.completion")]
    public string? Completion { get; set; }

    [JsonPropertyName("gen_ai.prompt")] 
    public string? Prompt { get; set; }
}