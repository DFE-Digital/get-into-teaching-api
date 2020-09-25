using Microsoft.PowerPlatform.Cds.Client;
using Newtonsoft.Json.Linq;

namespace GetIntoTeachingApiContractTests.Builders
{
    internal static class PickListItemBuilder
    {
        public static CdsServiceClient.PickListItem FromToken(JToken token)
        {
            var key = token["DisplayLabel"]!.ToString();
            var value = token["PickListItemId"]!.Value<int>();

            return new CdsServiceClient.PickListItem(key, value);
        }
    }
}