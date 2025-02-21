namespace Chapter7.ContentPreprocessing.Shared;

public class ValidationDataRecord
{
    public string Id { get; set; } = default!;
    public string FragmentId { get; set; } = default!;
    public string OriginalFileName { get; set; } = default!;
    public string Question { get; set; } = default!;
    public string Answer { get; set; } = default!;
    public string Context { get; set; } = default!;
}