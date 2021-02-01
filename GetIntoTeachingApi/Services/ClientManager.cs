using System.Collections.Generic;
using System.IO;
using System.Linq;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Utils;
using MoreLinq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace GetIntoTeachingApi.Services
{
    public class ClientManager : IClientManager
    {
        public virtual IEnumerable<Client> Clients { get; }
        private const string ClientsFile = "./Fixtures/clients.yml";
        private readonly IEnv _env;

        public ClientManager(IEnv env)
        {
            _env = env;
            Clients = LoadClients();
        }

        public Client GetClient(string apiKey)
        {
            return Clients.FirstOrDefault(c => c.ApiKey == apiKey);
        }

        private IEnumerable<Client> LoadClients()
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();
            var yaml = File.ReadAllText(ClientsFile);
            var clients = deserializer.Deserialize<List<Client>>(yaml);

            clients.ForEach(c => PopulateApiKey(c));

            return clients;
        }

        private void PopulateApiKey(Client client)
        {
            var environmentName = _env.EnvironmentName.ToUpper();
            client.ApiKey = _env.Get($"{client.ApiKeyPrefix}_API_KEY_{environmentName}");
        }
    }
}