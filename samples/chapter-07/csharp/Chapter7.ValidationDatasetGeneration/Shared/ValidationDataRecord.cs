namespace Chapter7.ValidationDatasetGeneration.ProcessingSteps;

public class ValidationDataRecord
{
    public string Id { get; set; } = default!;
    public string FragmentId { get; set; } = default!;
    public string Question { get; set; } = default!;
    public string Answer { get; set; } = default!;
}