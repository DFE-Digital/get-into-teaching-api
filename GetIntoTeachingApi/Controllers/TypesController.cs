using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Controllers
{
    [Route("api/types")]
    [ApiController]
    [Authorize(Policy = "SharedSecret")]
    public class TypesController : ControllerBase
    {
        private readonly ILogger<TypesController> _logger;

        public TypesController(ILogger<TypesController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("countries")]
        [SwaggerOperation(
            Summary = "Retrieves the list of countries.",
            OperationId = "GetCountryTypes",
            Tags = new[] { "Types" }
        )]
        public IActionResult GetCountries()
        {
            // TODO:
            return Ok(new[] { new Object() });
        }

        [HttpGet]
        [Route("teaching_subjects")]
        [SwaggerOperation(
            Summary = "Retrieves the list of teaching subjects.",
            OperationId = "GetTeachingSubjects",
            Tags = new[] { "Types" }
        )]
        public IActionResult GetTeachingSubjects()
        {
            // TODO:
            return Ok(new[] { new Object() });
        }
    }
}
