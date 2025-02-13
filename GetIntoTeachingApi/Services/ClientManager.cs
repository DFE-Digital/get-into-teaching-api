﻿using System.Collections.Generic;
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
        private const string ClientsFile = "./Config/clients.yml";
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

        private List<Client> LoadClients()
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
            client.ApiKey = _env.GetVariable($"{client.ApiKeyPrefix}_API_KEY");
        }
    }
}