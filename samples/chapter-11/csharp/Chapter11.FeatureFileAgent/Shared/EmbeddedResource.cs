namespace Chapter11.FeatureFileAgent.Shared;

public static class EmbeddedResource
{
    public static string Read(string name)
    {
        var fullyQualifiedName = $"Chapter11.FeatureFileAgent.{name}";

        var resourceContentStream = typeof(EmbeddedResource)
            .Assembly.GetManifestResourceStream(fullyQualifiedName);

        if (resourceContentStream == null)
        {
            throw new ArgumentException($"Resource '{fullyQualifiedName}' not found in assembly.");
        }

        using (var reader = new StreamReader(resourceContentStream))
        {
            return reader.ReadToEnd();
        }
    }
}