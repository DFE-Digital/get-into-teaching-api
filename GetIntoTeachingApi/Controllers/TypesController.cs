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
            IEnumerable<TypeEntity> countries = _crm.GetLookupItems("dfe_country");
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
        public IActionResult GetTeachingSubjects()
        {
            IEnumerable<TypeEntity> subjects = _crm.GetLookupItems("dfe_teachingsubjectlist");
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
        public IActionResult GetCandidateInitialTeacherTrainingYears()
        {
            IEnumerable<TypeEntity> years = _crm.GetPickListItems("contact", "dfe_ittyear");
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
        public IActionResult GetCandidatePreferredEducationPhases()
        {
            IEnumerable<TypeEntity> educationPhases = _crm.GetPickListItems("contact", "dfe_preferrededucationphase01");
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
        public IActionResult GetCandidateLocations()
        {
            IEnumerable<TypeEntity> locations = _crm.GetPickListItems("contact", "dfe_isinuk");
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
        public IActionResult GetQualificationDegreeStatus()
        {
            IEnumerable<TypeEntity> status = _crm.GetPickListItems("dfe_qualification", "dfe_degreestatus");
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
        public IActionResult GetQualificationCategories()
        {
            IEnumerable<TypeEntity> categories = _crm.GetPickListItems("dfe_qualification", "dfe_category");
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
        public IActionResult GetQualificationTypes()
        {
            IEnumerable<TypeEntity> types = _crm.GetPickListItems("dfe_qualification", "dfe_type");
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
        public IActionResult GetPastTeachingPositionEducationPhases()
        {
            IEnumerable<TypeEntity> educationPhases = _crm.GetPickListItems("dfe_candidatepastteachingposition", "dfe_educationphase");
            return Ok(educationPhases);
        }
    }
}
