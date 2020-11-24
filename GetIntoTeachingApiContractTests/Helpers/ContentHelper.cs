using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace GetIntoTeachingApiContractTests.Helpers
{
    public static class ContentHelper
    {
        public static StringContent GetStringContent(object obj)
            => new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
    }
}