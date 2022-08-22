using Xunit;

namespace GetIntoTeachingApiTests.Helpers
{
    [CollectionDefinition(nameof(NotThreadSafeResourceCollection), DisableParallelization = true)]
    public class NotThreadSafeResourceCollection { }
}
