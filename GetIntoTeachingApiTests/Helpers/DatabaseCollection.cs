using Xunit;

namespace GetIntoTeachingApiTests.Helpers
{
    [CollectionDefinition("Database")]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
    {
    }
}
