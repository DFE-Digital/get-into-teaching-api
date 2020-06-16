using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GetIntoTeachingApi.OperationFilters
{
    public class AuthOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var authRequired = context.ApiDescription.CustomAttributes()
                .Any(attr => attr.GetType() == typeof(AuthorizeAttribute));

            if (!authRequired)
            {
                return;
            }

            operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "apiKey" }
                        },
                        new List<string>()
                    }
                }
            };
        }
    }
}
