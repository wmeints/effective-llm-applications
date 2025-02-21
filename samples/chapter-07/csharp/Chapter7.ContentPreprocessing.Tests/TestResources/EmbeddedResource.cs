namespace Chapter7.ContentPreprocessing.Tests.TestResources;

public static class TestResource
{
    public static string Read(string resourceName)
    {
        var assembly = typeof(TestResource).Assembly;
        using var stream = assembly.GetManifestResourceStream($"Chapter7.ContentPreprocessing.Tests.TestResources.{resourceName}");
        using var reader = new StreamReader(stream!);
        return reader.ReadToEnd();
    }
}