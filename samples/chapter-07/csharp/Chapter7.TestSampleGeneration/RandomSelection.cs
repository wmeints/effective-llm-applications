namespace Chapter7.TestSampleGeneration;

public static class RandomSelection
{
    public static IEnumerable<T> SelectRandom<T>(this IEnumerable<T> source, int count)
    {
        var random = new Random();
        return source.OrderBy(x => random.Next()).Take(count);
    }
}
