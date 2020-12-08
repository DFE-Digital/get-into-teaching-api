using System.Threading.Tasks;

namespace GetIntoTeachingApi.Services
{
    public interface IRedisService
    {
        Task<string> CheckStatusAsync();
    }
}