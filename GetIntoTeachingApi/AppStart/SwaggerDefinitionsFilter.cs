using System;
using System.Collections.Generic;
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
                .Where(assembly => assembly.FullName.StartsWith("GetIntoTeachingApi"))
                .SelectMany(assembly => assembly.GetTypes())
                .ToList();

            IEnumerable<string> swaggerIngoredTypeNames = apiTypes
                .Where(type => Attribute.IsDefined(type, typeof(SwaggerIgnoreAttribute)))
                .Select(type => type.Name);

            var schemas = swaggerDoc.Components.Schemas;

            foreach (var definition in schemas)
            {
                if (swaggerIngoredTypeNames.Contains(definition.Key))
                {
                    schemas.Remove(definition);
                }
            }
        }
    }
}
