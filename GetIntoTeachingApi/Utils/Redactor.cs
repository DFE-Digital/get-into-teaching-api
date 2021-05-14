using System.Collections.Generic;
using MoreLinq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GetIntoTeachingApi.Utils
{
    public static class Redactor
    {
        private static readonly string _redactedValue = "******";
        private static readonly IEnumerable<string> _sensitivePropertyNames = new string[]
        {
            "password",
            "email",
            "secondaryEmail",
            "fullName",
            "firstName",
            "lastName",
            "telephone",
            "secondaryTelephone",
            "addressTelephone",
            "mobileTelephone",
            "dateOfBirth",
            "teacherId",
            "addressLine1",
            "addressLine2",
            "addressLine3",
        };

        public static string RedactJson(string json)
        {
            try
            {
                var rootToken = JToken.Parse(json);
                var jsonPath = $"$..['{string.Join("','", _sensitivePropertyNames)}']";

                rootToken
                    .SelectTokens(jsonPath)
                    .ForEach(t => t.Replace(new JValue(_redactedValue)));

                return JsonConvert.SerializeObject(rootToken);
            }
            catch (JsonReaderException)
            {
                return string.Empty;
            }
        }
    }
}
