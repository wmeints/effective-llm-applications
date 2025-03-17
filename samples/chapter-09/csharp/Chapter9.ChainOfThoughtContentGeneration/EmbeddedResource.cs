namespace Chapter9.ChainOfThoughtContentGeneration;

public static class EmbeddedResource
{
    public static string Read(string name)
    {
        var assembly = typeof(EmbeddedResource).Assembly;
        var resourceName = $"{typeof(EmbeddedResource).Namespace}.{name}";

        using var stream = assembly.GetManifestResourceStream(resourceName);
        using var reader = new StreamReader(stream!);

        return reader.ReadToEnd();
    }
}