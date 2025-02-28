namespace Chapter7.TestSampleGeneration;

public static class EmbeddedResource
{
    public static string Read(string resourceName)
    {
        var assembly = typeof(EmbeddedResource).Assembly;
        using var stream = assembly.GetManifestResourceStream($"Chapter7.TestSampleGeneration.{resourceName}");
        using var reader = new StreamReader(stream!);
        return reader.ReadToEnd();
    }
}