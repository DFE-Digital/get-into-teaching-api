using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Models;
using Simple.OData.Client;

namespace GetIntoTeachingApi.Services.Crm
{
    public class WebApiClient : IWebApiClient
    {
        private readonly IODataClient _client;
        private readonly IWebApiClientCache _cache;
        private static DateTime CacheExpiry => DateTime.Now.AddHours(3);
        private const int MaximumNumberOfPrivacyPolicies = 3;
        private const int MaximumNumberOfCandidatesToMatch = 20;

        public WebApiClient(IWebApiClientCache cache, IODataClient client)
        {
            _client = client;
            _cache = cache;
        }

        public async Task<IEnumerable<TypeEntity>> GetLookupItems(Lookup lookup)
        {
            return await _cache.GetOrCreateAsync(lookup.CacheKey, CacheExpiry, async () =>
            {
                var id = lookup.IdAttribute;
                var results = await _client.For(lookup.EntityName).Select(id, "dfe_name").FindEntriesAsync();

                return results.Select(item => new TypeEntity() { Id = item[id], Value = item["dfe_name"] });
            });
        }

        public async Task<IEnumerable<TypeEntity>> GetOptionSetItems(OptionSet optionSet)
        {
            return await _cache.GetOrCreateAsync(optionSet.CacheKey, CacheExpiry, async () =>
            {
                var query = $"EntityDefinitions({optionSet.EntityMetadataId})/Attributes({optionSet.AttributeMetadataId})/" +
                            $"Microsoft.Dynamics.CRM.PicklistAttributeMetadata?$select=OptionSet&$expand=OptionSet";
                dynamic result = await _client.FindEntryAsync(query);
                var options = (dynamic[]) result["Options"];

                return options.Select(item => new TypeEntity() { Id = item["Value"], Value = ExtractLabel(item) });
            });
        }

        public async Task<PrivacyPolicy> GetLatestPrivacyPolicy()
        {
            return (await GetPrivacyPolicies()).FirstOrDefault();
        }

        public async Task<IEnumerable<PrivacyPolicy>> GetPrivacyPolicies()
        {
            return await _cache.GetOrCreateAsync("dfe_privacypolicy", CacheExpiry, 
                async () => await _client.For<PrivacyPolicy>().Top(MaximumNumberOfPrivacyPolicies)
                    .Filter(p => p.IsActive && p.Type == (int) PrivacyPolicy.Types.Web)
                    .OrderByDescending(p => p.CreatedAt).FindEntriesAsync());
        }

        public async Task<Candidate> GetCandidate(ExistingCandidateRequest request)
        {
            var candidates = await _client.For<Candidate>().Top(MaximumNumberOfCandidatesToMatch)
                .Expand(c => c.PastTeachingPositions)
                .Expand(c => c.Qualifications)
                .Expand("dfe_contact_dfe_candidatepastteachingposition_ContactId/dfe_SubjectTaught")
                .Expand(c => c.PreferredTeachingSubject)
                .Filter(c => c.Email == request.Email) // Will perform a case-insensitive comparison
                .OrderByDescending(c => c.CreatedAt).FindEntriesAsync();

            return candidates.FirstOrDefault(request.Match);
        }

        public async Task<T> Upsert<T>(T model) where T : BaseModel
        {
            var result = await Upsert(new[] { model });
            return result.First();
        }

        public async Task<IEnumerable<T>> Upsert<T>(IEnumerable<T> models) where T : BaseModel
        {
            var result = new List<T>();

            foreach (var model in models)
            {
                if (model.Id != null)
                    result.Add((await _client.For<T>().Set(model).UpdateEntryAsync()));
                else
                    result.Add(await _client.For<T>().Set(model).InsertEntryAsync());
            }

            return result;
        }

        public static ODataClient CreateODataClient(IODataCredentials credentials, IAccessTokenProvider tokenProvider)
        {
            var settings = new ODataClientSettings(credentials.ODataServiceUri())
            {
                IgnoreResourceNotFoundException = true, 
                IgnoreUnmappedProperties = true
            };

            settings.BeforeRequestAsync += async delegate (HttpRequestMessage message)
            {
                var accessToken = await tokenProvider.GetAccessTokenAsync(credentials);
                message.Headers.Add("Authorization", $"Bearer {accessToken}");
                message.Headers.Add("OData-MaxVersion", "4.0");
                message.Headers.Add("OData-Version", "4.0");

                if (message.Method == HttpMethod.Post)
                    StripNullsFromMessageBody(message);
            };

            var client = new ODataClient(settings);

            // Cache metadata on startup (otherwise it will be
            // done as part of the first request, slowing it down).
            client.GetMetadataAsync().Wait();

            return client;
        }

        private static async void StripNullsFromMessageBody(HttpRequestMessage message)
        {
            // We never need to send null values to the CRM and Simple OData Client
            // sends a null id on inserts - Dynamics doesn't like this, so we strip them out.
            var content = await message.Content.ReadAsStringAsync();
            content = Regex.Replace(content, @"""\w+?""\s*:\s*null,?", "");
            message.Content = new StringContent(content, Encoding.UTF8, "application/json");
        }

        private static string ExtractLabel(dynamic option)
        {
            return option["Label"]["LocalizedLabels"][0]["Label"];
        }
    }
}
