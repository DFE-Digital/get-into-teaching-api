using System.Collections.Generic;
using GetIntoTeachingApi.Models;

namespace GetIntoTeachingApi.Services
{
    public interface IClientManager
    {
        IEnumerable<Client> Clients { get; }
        Client GetClient(string apiKey);
    }
}