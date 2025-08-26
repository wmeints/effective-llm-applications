using System.Runtime.CompilerServices;
using Polly;
using Polly.Retry;

namespace Chapter11.FeatureFileAgent.Shared;

public static class PollyStreamingExtensions
{
    public static async IAsyncEnumerable<TItem> ExecuteEnumerableAsync<TItem>(
        this ResiliencePipeline policy,
        Func<CancellationToken, IAsyncEnumerable<TItem>> action,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var (enumerator, movedNext) = await policy.ExecuteAsync(
            async (ct) =>
            {
                var asyncEnumerable = action(ct);
                var asyncEnumerator = asyncEnumerable.GetAsyncEnumerator(ct);

                return (asyncEnumerator, await asyncEnumerator.MoveNextAsync());
            },
            cancellationToken);

        if (movedNext)
        {
            do
            {
                yield return enumerator.Current;
            }
            while (await enumerator.MoveNextAsync());
        }
    }
}