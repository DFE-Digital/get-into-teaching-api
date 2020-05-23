using System.Threading.Tasks;
using GetIntoTeachingApi.Services.Crm;

namespace GetIntoTeachingApi.Adapters
{
    public interface IAccessTokenProvider
    {
        Task<string> GetAccessTokenAsync(IODataCredentials credentials);
    }
}
