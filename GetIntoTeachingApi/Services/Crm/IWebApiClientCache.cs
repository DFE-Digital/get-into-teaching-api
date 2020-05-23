using System;
using System.Threading.Tasks;

namespace GetIntoTeachingApi.Services.Crm
{
    public interface IWebApiClientCache
    {
        Task<TItem> GetOrCreateAsync<TItem>(string key, DateTime expiresAt, Func<Task<TItem>> create);
    }
}
