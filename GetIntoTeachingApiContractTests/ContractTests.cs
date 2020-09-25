using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using GetIntoTeachingApi;
using GetIntoTeachingApiContractTests.Assertions;
using GetIntoTeachingApiContractTests.Attributes;
using GetIntoTeachingApiContractTests.Fixtures;
using GetIntoTeachingApiContractTests.Servers;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using Xunit;

namespace GetIntoTeachingApiContractTests
{
    public class ContractTests : IClassFixture<ContractTestFixture<Startup>>
    {
        private readonly HttpClient _client;

        public ContractTests(ContractTestFixture<Startup> fixture)
        {
            _client = fixture.Client;
        }

        [Theory]
        [JsonContractTestData("./contracts")]
        public async Task ThroughApiService(string filename, StringContent requestBody, Entity contact, string filepath)
        {
            // Act
            var response = await _client.PostAsync("/api/teacher_training_adviser/candidates", requestBody);
            var responseBody = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            
            // if ((int)response.StatusCode > 299) return;
            
            // Assert
            var crmContact = await ServerUnderTest.CrmServiceAdapter.GetCandidateRequests();
            
            SaveData(filepath, crmContact);
            
            crmContact.Should().Match(contact);
        }

        private static void SaveData(string filename, Entity data)
        {
            var outFileName = filename.Replace(".json", "_crm.json");
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            
            File.WriteAllText(outFileName, json);
        }
    }
}
