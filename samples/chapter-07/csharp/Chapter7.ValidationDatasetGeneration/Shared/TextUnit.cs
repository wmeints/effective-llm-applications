using Microsoft.Extensions.VectorData;

namespace Chapter7.ContentPreprocessing.Shared;

public class TextUnit
{
    [VectorStoreRecordKey]
    public required ulong Id { get; set; }

    [VectorStoreRecordData(IsFilterable = true)]
    public required string ChunkId { get; set; }

    [VectorStoreRecordData(IsFullTextSearchable = true)]
    public required string Content { get; set; }

    [VectorStoreRecordData(IsFilterable = true)]
    public required string OriginalFileName { get; set; }

    [VectorStoreRecordVector(1536)]
    public ReadOnlyMemory<float> Embedding { get; set; }
}