using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit.Sdk;

namespace GetIntoTeachingApiTests.Helpers
{
    public class ContractTestInputsAttribute : DataAttribute
    {
        private readonly string _directory;

        public ContractTestInputsAttribute(string directory)
        {
            _directory = directory;
        }

        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            if (testMethod == null) {
                throw new ArgumentNullException(nameof(testMethod));
            }

            if (!Directory.Exists(_directory))
            {
                throw new ArgumentException($"Could not find directory: {_directory}");
            }

            var files = Directory.GetFiles(_directory, "*.json");

            return files.Select(file => {
                var scenario = Path.GetFileNameWithoutExtension(file).Replace("_", " ");
                return new string[] { scenario };
            });
        }
    }
}
