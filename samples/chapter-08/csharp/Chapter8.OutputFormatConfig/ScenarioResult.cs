public class ScenarioResult
{
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public List<string> Steps { get; set; } = new();
}