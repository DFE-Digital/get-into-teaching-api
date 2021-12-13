using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GetIntoTeachingApi.Attributes;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GetIntoTeachingApi.AppStart
{
    public class SwaggerDefinitionsFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var apiTypes = AppDomain
                .CurrentDomain
                .GetAssemblies()
                .Where(assembly => assembly.FullName.StartsWith("GetIntoTeachingApi", true, CultureInfo.CurrentCulture))
                .SelectMany(assembly => assembly.GetTypes())
                .ToList();

            IEnumerable<string> swaggerIngoredTypeNames = apiTypes
                .Where(type => Attribute.IsDefined(type, typeof(SwaggerIgnoreAttribute)))
                .Select(type => type.Name);

            var schemas = swaggerDoc.Components.Schemas;

            foreach (var schema in schemas.Where(s => swaggerIngoredTypeNames.Contains(s.Key)))
            {
                schemas.Remove(schema);
            }
        }
    }
}
