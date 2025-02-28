using System;
using Microsoft.Extensions.VectorData;

namespace Chapter7.TestSampleGeneration
{
    public class TextUnit
    {
        [VectorStoreRecordKey]
        public ulong Id { get; set; }

        [VectorStoreRecordData(IsFilterable = true)]
        public string OriginalFileName { get; set; } = default!;

        [VectorStoreRecordData(IsFullTextSearchable = true)]
        public string Content { get; set; } = default!;

        [VectorStoreRecordVector(Dimensions: 1536, DistanceFunction.CosineSimilarity, IndexKind.Hnsw)]
        public ReadOnlyMemory<float> Embedding { get; set; }
    }
}