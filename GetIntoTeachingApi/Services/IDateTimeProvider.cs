using System;

namespace GetIntoTeachingApi.Services
{
    public interface IDateTimeProvider
    {
        DateTime UtcNow { get; }
    }
}
