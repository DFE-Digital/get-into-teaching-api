using System.Threading.Tasks;
using GetIntoTeachingApi.Services.Crm;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace GetIntoTeachingApi.Adapters
{
    public class AccessTokenProvider : IAccessTokenProvider
    {
        public async Task<string> GetAccessTokenAsync(IODataCredentials credentials)
        {
            var authContext = new AuthenticationContext(credentials.AuthUrl());
            var clientCredential = new ClientCredential(credentials.ClientId(), credentials.Secret());
            var result = await authContext.AcquireTokenAsync(credentials.ServiceUrl(), clientCredential);

            return result.AccessToken;
        }
    }
}
