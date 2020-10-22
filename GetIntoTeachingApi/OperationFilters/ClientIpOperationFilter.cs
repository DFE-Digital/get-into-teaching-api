using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GetIntoTeachingApi.OperationFilters
{
    public class ClientIpOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var parameter = new OpenApiParameter()
            {
                Name = "X-Client-IP",
                In = ParameterLocation.Header,
                Description = "IP address of the end user or client " +
                "application used for rate limiting. Will fall into a globally " +
                "rate limited bucket if not specified.",
                Required = false,
            };

            operation.Parameters.Add(parameter);
        }
    }
}
