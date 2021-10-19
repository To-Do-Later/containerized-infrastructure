using Xunit;

namespace TestProject.Fixtures
{
    [CollectionDefinition("InfrastructureWithExistingRecordsCollection")]
    public class InfrastructureWithExistingRecordsCollection : ICollectionFixture<InfrastructureWithExistingRecords>
    {

    }
}
