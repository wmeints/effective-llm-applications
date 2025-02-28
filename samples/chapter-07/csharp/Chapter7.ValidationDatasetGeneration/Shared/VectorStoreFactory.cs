using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Qdrant.Client;

namespace Chapter7.ValidationDatasetGeneration.Shared;

public class VectorStoreFactory
{
    public static IVectorStore CreateVectorStore(IConfiguration configuration)
    {
        var vectorStoreConfiguration = configuration.GetSection("VectorStore").Get<VectorStoreConfiguration>();
        var vectorStoreClient = new QdrantClient(vectorStoreConfiguration!.HostName);
        var vectorStore = new QdrantVectorStore(vectorStoreClient);

        return vectorStore;
    }
}