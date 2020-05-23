using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services.Crm;
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
        private readonly IWebApiClient _client;

        public TypesController(ILogger<TypesController> logger, IWebApiClient client)
        {
            _logger = logger;
            _client = client;
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
            var countries = await _client.GetLookupItems(Lookup.Country);
            return Ok(countries);
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
            var subjects = await _client.GetLookupItems(Lookup.TeachingSubject);
            return Ok(subjects);
        }

        [HttpGet]
        [Route("candidate/initial_teacher_training_years")]
        [SwaggerOperation(
            Summary = "Retrieves the list of candidate initial teacher training years.",
            OperationId = "GetCandidateInitialTeacherTrainingYears",
            Tags = new[] { "Types" }
        )]
        [ProducesResponseType(typeof(IEnumerable<TypeEntity>), 200)]
        public async Task<IActionResult> GetCandidateInitialTeacherTrainingYears()
        {
            var years = await _client.GetOptionSetItems(OptionSet.CandidateInitialTeacherTrainingYears);
            return Ok(years);
        }

        [HttpGet]
        [Route("candidate/preferred_education_phases")]
        [SwaggerOperation(
            Summary = "Retrieves the list of candidate preferred education phases.",
            OperationId = "GetCandidatePreferredEducationPhases",
            Tags = new[] { "Types" }
        )]
        [ProducesResponseType(typeof(IEnumerable<TypeEntity>), 200)]
        public async Task<IActionResult> GetCandidatePreferredEducationPhases()
        {
            var educationPhases = await _client.GetOptionSetItems(OptionSet.CandidatePreferredEducationPhases);
            return Ok(educationPhases);
        }

        [HttpGet]
        [Route("candidate/locations")]
        [SwaggerOperation(
            Summary = "Retrieves the list of candidate locations.",
            OperationId = "GetCandidateLocations",
            Tags = new[] { "Types" }
        )]
        [ProducesResponseType(typeof(IEnumerable<TypeEntity>), 200)]
        public async Task<IActionResult> GetCandidateLocations()
        {
            var locations = await _client.GetOptionSetItems(OptionSet.CandidateLocations);
            return Ok(locations);
        }

        [HttpGet]
        [Route("qualification/degree_status")]
        [SwaggerOperation(
            Summary = "Retrieves the list of qualification degree status.",
            OperationId = "GetQualificationDegreeStatus",
            Tags = new[] { "Types" }
        )]
        [ProducesResponseType(typeof(IEnumerable<TypeEntity>), 200)]
        public async Task<IActionResult> GetQualificationDegreeStatus()
        {
            var status = await _client.GetOptionSetItems(OptionSet.QualificationDegreeStatus);
            return Ok(status);
        }

        [HttpGet]
        [Route("qualification/categories")]
        [SwaggerOperation(
            Summary = "Retrieves the list of qualification categories.",
            OperationId = "GetQualificationCategories",
            Tags = new[] { "Types" }
        )]
        [ProducesResponseType(typeof(IEnumerable<TypeEntity>), 200)]
        public async Task<IActionResult> GetQualificationCategories()
        {
            var categories = await _client.GetOptionSetItems(OptionSet.QualificationCategories);
            return Ok(categories);
        }

        [HttpGet]
        [Route("qualification/types")]
        [SwaggerOperation(
            Summary = "Retrieves the list of qualification types.",
            OperationId = "GetQualificationTypes",
            Tags = new[] { "Types" }
        )]
        [ProducesResponseType(typeof(IEnumerable<TypeEntity>), 200)]
        public async Task<IActionResult> GetQualificationTypes()
        {
            var types = await _client.GetOptionSetItems(OptionSet.QualificationTypes);
            return Ok(types);
        }

        [HttpGet]
        [Route("past_teaching_position/education_phases")]
        [SwaggerOperation(
            Summary = "Retrieves the list of past teaching position education phases.",
            OperationId = "GetPastTeachingPositionEducationPhases",
            Tags = new[] { "Types" }
        )]
        [ProducesResponseType(typeof(IEnumerable<TypeEntity>), 200)]
        public async Task<IActionResult> GetPastTeachingPositionEducationPhases()
        {
            var educationPhases = await _client.GetOptionSetItems(OptionSet.PastTeachingPositionEducationPhases);
            return Ok(educationPhases);
        }

        [HttpGet]
        [Route("teaching_event/types")]
        [SwaggerOperation(
            Summary = "Retrieves the list of teaching event types.",
            OperationId = "GetTeachingEventTypes",
            Tags = new[] { "Types" }
        )]
        [ProducesResponseType(typeof(IEnumerable<TypeEntity>), 200)]
        public async Task<IActionResult> GetTeachingEventTypes()
        {
            var eventTypes = await _client.GetOptionSetItems(OptionSet.TeachingEventTypes);
            return Ok(eventTypes);
        }
    }
}
