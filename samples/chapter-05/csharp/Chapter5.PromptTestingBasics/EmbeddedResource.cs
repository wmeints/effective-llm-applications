namespace Chapter5.PromptTestingBasics;

using System.Reflection;
using System.IO;

public static class EmbeddedResource
{
    public static string Read(string name)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"Chapter5.PromptTestingBasics.{name}";

        using var stream = assembly.GetManifestResourceStream(resourceName);
        
        if (stream == null)
        {
            throw new Exception($"Resource {resourceName} not found");
        }

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}