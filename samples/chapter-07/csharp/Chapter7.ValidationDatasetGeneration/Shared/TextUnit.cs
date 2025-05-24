using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Data;

namespace Chapter7.ValidationDatasetGeneration.Shared;

public class TextUnit
{
    [VectorStoreKey]
    public ulong Id { get; set; }

    [VectorStoreData]
    public string OriginalFileName { get; set; } = default!;

    [VectorStoreData(IsFullTextIndexed = true)]
    public string Content { get; set; } = default!;

    [VectorStoreVector(1536, DistanceFunction = DistanceFunction.CosineSimilarity, IndexKind = IndexKind.Hnsw)]
    public ReadOnlyMemory<float> Embedding { get; set; }
}