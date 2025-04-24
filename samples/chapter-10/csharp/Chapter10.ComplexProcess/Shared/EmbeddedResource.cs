namespace Chapter10.SimpleProcess.Shared;

public static class EmbeddedResource
{
    public static string Read(string name)
    {
        var assembly = typeof(EmbeddedResource).Assembly;
        var resourceName = $"Chapter10.ComplexProcess.{name}";

        using var stream = assembly.GetManifestResourceStream(resourceName);
        using var reader = new StreamReader(stream!);

        return reader.ReadToEnd();
    }
}