namespace Chapter7.FaithfulnessMetric;

public static class EmbeddedResource
{
    public static string Read(string resourceName)
    {
        var assembly = typeof(EmbeddedResource).Assembly;
        using var stream = assembly.GetManifestResourceStream($"Chapter7.FaithfulnessMetric.{resourceName}");
        using var reader = new StreamReader(stream!);
        return reader.ReadToEnd();
    }
}