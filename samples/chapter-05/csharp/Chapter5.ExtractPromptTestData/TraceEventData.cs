public class TraceEventData
{
    public required string Message { get; set; }
    public required TraceProperties Properties { get; set; }
    public DateTime TimeGenerated { get; set; }
}