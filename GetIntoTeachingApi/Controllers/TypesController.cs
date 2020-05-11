using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace GetIntoTeachingApi.Controllers
{
    [Route("api/types")]
    [ApiController]
    [Authorize(Policy = "SharedSecret")]
    public class TypesController : ControllerBase
    {
        private readonly ILogger<TypesController> _logger;
        private readonly ICrmService _crm;

        public TypesController(ILogger<TypesController> logger, ICrmService crm)
        {
            _logger = logger;
            _crm = crm;
        }

        [HttpGet]
        [Route("countries")]
        [SwaggerOperation(
            Summary = "Retrieves the list of countries.",
            OperationId = "GetCountryTypes",
            Tags = new[] { "Types" }
        )]
        [ProducesResponseType(typeof(IEnumerable<TypeEntity>), 200)]
        public async Task<IActionResult> GetCountries()
        {
            IEnumerable<TypeEntity> countryTypes = await _crm.GetCountries();
            return Ok(countryTypes);
        }

        [HttpGet]
        [Route("teaching_subjects")]
        [SwaggerOperation(
            Summary = "Retrieves the list of teaching subjects.",
            OperationId = "GetTeachingSubjects",
            Tags = new[] { "Types" }
        )]
        [ProducesResponseType(typeof(IEnumerable<TypeEntity>), 200)]
        public async Task<IActionResult> GetTeachingSubjects()
        {
            IEnumerable<TypeEntity> teachingSubjectTypes = await _crm.GetTeachingSubjects();
            return Ok(teachingSubjectTypes);
        }
    }
}
