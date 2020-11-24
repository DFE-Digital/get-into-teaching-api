using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using Castle.Core.Internal;
using GetIntoTeachingApiContractTests.Builders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit.Sdk;

namespace GetIntoTeachingApiContractTests.Attributes
{
    public class JsonContractTestDataAttribute : DataAttribute
    {
        private readonly string _contractsFolderPath;

        /// <summary>
        /// Load data from a directory of JSON contract files as the data sources for a theory
        /// </summary>
        /// <param name="contractsFolderPath">The relative path to the JSON contract files to load</param>
        public JsonContractTestDataAttribute(string contractsFolderPath)
        {
            _contractsFolderPath = contractsFolderPath;
        }

        /// <inheritDoc />
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            if (testMethod == null) { 
              throw new ArgumentNullException(nameof(testMethod));
            }

            var currentDir = Path.GetFullPath("../../../", Directory.GetCurrentDirectory());
            
            // Get the absolute path to the JSON file
            var path = Path.GetFullPath(_contractsFolderPath, currentDir) + "\\";

            if (!Directory.Exists(path))
            {
                throw new ArgumentException($"Could not find contract data directory at path: {path}");
            }
            
            // get a list of candidate data files
            var cases = Directory.GetFiles(path, "candidate_*.json")
                .Where(x => !x.Contains("_crm"))
                .Select(filepath =>
                {
                    var fileData = File.ReadAllText(filepath);
                    var data = JsonConvert.DeserializeObject<JObject>(fileData);

                    var apiSubmission = (JObject)data["apiSubmission"];
                    if (apiSubmission == null)
                    {
                        return null;
                    }

                    var phoneCallScheduledAt = apiSubmission["phoneCallScheduledAt"]?.ToString();
                    if (phoneCallScheduledAt != null 
                        && phoneCallScheduledAt.Equals("0000-00-00T00:00:00Z"))
                    {
                        apiSubmission["phoneCallScheduledAt"] = DateTime.UtcNow.AddHours(1).ToString("yyyy-MM-ddTHH:mm:sszzz");;
                    }

                    var contactData = data["dynamicsContactEntity"];
                    
                    // Arrange
                    return new object[]
                    {
                        Path.GetFileName(filepath),
                        new StringContent(apiSubmission.ToString(), Encoding.UTF8, "application/json"),
                        contactData != null ? CrmEntityBuilder.FromToken(contactData) : null,
                        filepath
                    };
                })
                .Where(x => !x.IsNullOrEmpty())
                .ToList();

            return cases;
        }
    }
}