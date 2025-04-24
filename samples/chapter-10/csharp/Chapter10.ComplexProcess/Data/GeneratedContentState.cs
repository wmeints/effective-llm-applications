namespace Chapter10.ComplexProcess.Data;

public class GeneratedContentState
{
    public List<SectionContent> GeneratedSections { get; set; } = new();
    public Queue<string> PendingSections { get; set; } = new();
    public string Topic { get; set; } = default!;
    public string Title { get; set; } = default!;
    public ContentGenerationStatus Status { get; set; } = ContentGenerationStatus.Pending;
}