using System;

namespace GetIntoTeachingApi.Services
{
    public interface ICrmCache
    {
        TItem GetOrCreate<TItem>(string key, DateTime expiresAt, Func<TItem> create);
    }
}
