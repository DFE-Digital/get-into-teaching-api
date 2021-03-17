using System.Threading.Tasks;
using StackExchange.Redis;

namespace GetIntoTeachingApi.Services
{
    public interface IRedisService
    {
        Task<string> CheckStatusAsync();
        IDatabase Database { get; }
    }
}