namespace Chapter10.IntelligentRequestRouting.Shared;

public static class EmbeddedResource
{
    public static string Read(string name)
    {
        var assembly = typeof(EmbeddedResource).Assembly;
        var resourceName = $"Chapter10.IntelligentRequestRouting.{name}";

        using var stream = assembly.GetManifestResourceStream(resourceName);
        using var reader = new StreamReader(stream!);

        return reader.ReadToEnd();
    }
}