using System.Reflection;

namespace Chapter6.PromptBasedFunctions;

public static class EmbeddedResource
{
    public static string Read(string name)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"Chapter6.PromptBasedFunctions.{name}";

        using var stream = assembly.GetManifestResourceStream(resourceName);

        if (stream == null)
        {
            throw new Exception($"Resource {resourceName} not found");
        }

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}