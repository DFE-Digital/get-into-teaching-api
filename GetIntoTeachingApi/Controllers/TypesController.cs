using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
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
        public IActionResult GetCountries()
        {
            IEnumerable<TypeEntity> countryTypes = _crm.GetLookupItems("dfe_country");
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
        public IActionResult GetTeachingSubjects()
        {
            IEnumerable<TypeEntity> teachingSubjectTypes = _crm.GetLookupItems("dfe_teachingsubjectlist");
            return Ok(teachingSubjectTypes);
        }


        [HttpGet]
        [Route("candidate/initial_teacher_training_years")]
        [SwaggerOperation(
            Summary = "Retrieves the list of candidate initial teacher training years.",
            OperationId = "GetCandidateInitialTeacherTrainingYears",
            Tags = new[] { "Types" }
        )]
        [ProducesResponseType(typeof(IEnumerable<TypeEntity>), 200)]
        public IActionResult GetCandidateInitialTeacherTrainingYears()
        {
            IEnumerable<TypeEntity> initialTeacherTrainingYears = _crm.GetPickListItems("contact", "dfe_ittyear");
            return Ok(initialTeacherTrainingYears);
        }

        [HttpGet]
        [Route("candidate/preferred_education_phases")]
        [SwaggerOperation(
            Summary = "Retrieves the list of candidate preferred education phases.",
            OperationId = "GetCandidatePreferredEducationPhases",
            Tags = new[] { "Types" }
        )]
        [ProducesResponseType(typeof(IEnumerable<TypeEntity>), 200)]
        public IActionResult GetCandidatePreferredEducationPhases()
        {
            IEnumerable<TypeEntity> preferredEducationPhases = _crm.GetPickListItems("contact", "dfe_preferrededucationphase01");
            return Ok(preferredEducationPhases);
        }

        [HttpGet]
        [Route("candidate/location")]
        [SwaggerOperation(
            Summary = "Retrieves the list of candidate locations.",
            OperationId = "GetCandidateLocations",
            Tags = new[] { "Types" }
        )]
        [ProducesResponseType(typeof(IEnumerable<TypeEntity>), 200)]
        public IActionResult GetCandidateLocations()
        {
            IEnumerable<TypeEntity> locations = _crm.GetPickListItems("contact", "dfe_isinuk");
            return Ok(locations);
        }

        [HttpGet]
        [Route("qualification/degree_status")]
        [SwaggerOperation(
            Summary = "Retrieves the list of qualification degree status.",
            OperationId = "GetCandidateQualifications",
            Tags = new[] { "Types" }
        )]
        [ProducesResponseType(typeof(IEnumerable<TypeEntity>), 200)]
        public IActionResult GetQualificationDegreeStatus()
        {
            IEnumerable<TypeEntity> status = _crm.GetPickListItems("dfe_qualification", "dfe_degreestatus");
            return Ok(status);
        }
    }
}
