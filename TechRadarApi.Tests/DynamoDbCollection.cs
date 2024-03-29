using Xunit;

namespace TechRadarApi.Tests
{
    [CollectionDefinition("DynamoDb Collection", DisableParallelization = true)]
    public class DynamoDbCollection : ICollectionFixture<AwsMockWebApplicationFactory<Startup>>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
