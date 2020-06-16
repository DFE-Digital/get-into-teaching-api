using System.Collections.Generic;
using System.Linq;
using GetIntoTeachingApi.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Controllers
{
    [Route("api/operations")]
    [ApiController]
    public class OperationsController : ControllerBase
    {
        [HttpGet]
        [Route("generate_mapping_info")]
        [SwaggerOperation(
            Summary = "Generates the mapping information.",
            Description = "Generates the mapping information describing how the " +
                          "models in the API map to the corresponding entities in Dynamics 365.",
            OperationId = "GenerateMappingInfo",
            Tags = new[] { "Operations" })]
        [ProducesResponseType(typeof(IEnumerable<MappingInfo>), 200)]
        public IActionResult GenerateMappingInfo()
        {
            var assembly = typeof(BaseModel).Assembly;
            var subTypes = assembly.GetTypes().Where(t => t.BaseType == typeof(BaseModel));
            var mappings = subTypes.Select(s => new MappingInfo(s));

            return Ok(mappings.ToList());
        }
    }
}