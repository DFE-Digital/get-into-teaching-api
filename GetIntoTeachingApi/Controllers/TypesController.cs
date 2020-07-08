using System.Collections.Generic;
using System.Threading.Tasks;
using GetIntoTeachingApi.Filters;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Controllers
{
    [Route("api/types")]
    [ApiController]
    [Authorize]
    public class TypesController : ControllerBase
    {
        private readonly IStore _store;

        public TypesController(IStore store)
        {
            _store = store;
        }

        [HttpGet]
        [CrmETag]
        [Route("countries")]
        [SwaggerOperation(
            Summary = "Retrieves the list of countries.",
            OperationId = "GetCountryTypes",
            Tags = new[] { "Types" })]
        [ProducesResponseType(typeof(IEnumerable<TypeEntity>), 200)]
        public async Task<IActionResult> GetCountries()
        {
            return Ok(await _store.GetLookupItems("dfe_country").ToListAsync());
        }

        [HttpGet]
        [CrmETag]
        [Route("teaching_subjects")]
        [SwaggerOperation(
            Summary = "Retrieves the list of teaching subjects.",
            OperationId = "GetTeachingSubjects",
            Tags = new[] { "Types" })]
        [ProducesResponseType(typeof(IEnumerable<TypeEntity>), 200)]
        public async Task<IActionResult> GetTeachingSubjects()
        {
            return Ok(await _store.GetLookupItems("dfe_teachingsubjectlist").ToListAsync());
        }

        [HttpGet]
        [CrmETag]
        [Route("candidate/initial_teacher_training_years")]
        [SwaggerOperation(
            Summary = "Retrieves the list of candidate initial teacher training years.",
            OperationId = "GetCandidateInitialTeacherTrainingYears",
            Tags = new[] { "Types" })]
        [ProducesResponseType(typeof(IEnumerable<TypeEntity>), 200)]
        public async Task<IActionResult> GetCandidateInitialTeacherTrainingYears()
        {
            return Ok(await _store.GetPickListItems("contact", "dfe_ittyear").ToListAsync());
        }

        [HttpGet]
        [CrmETag]
        [Route("candidate/preferred_education_phases")]
        [SwaggerOperation(
            Summary = "Retrieves the list of candidate preferred education phases.",
            OperationId = "GetCandidatePreferredEducationPhases",
            Tags = new[] { "Types" })]
        [ProducesResponseType(typeof(IEnumerable<TypeEntity>), 200)]
        public async Task<IActionResult> GetCandidatePreferredEducationPhases()
        {
            return Ok(await _store.GetPickListItems("contact", "dfe_preferrededucationphase01").ToListAsync());
        }

        [HttpGet]
        [CrmETag]
        [Route("candidate/channels")]
        [SwaggerOperation(
            Summary = "Retrieves the list of candidate channels.",
            OperationId = "GetCandidateChannels",
            Tags = new[] { "Types" })]
        [ProducesResponseType(typeof(IEnumerable<TypeEntity>), 200)]
        public async Task<IActionResult> GetCandidateChannels()
        {
            return Ok(await _store.GetPickListItems("contact", "dfe_channelcreation").ToListAsync());
        }

        [HttpGet]
        [CrmETag]
        [Route("candidate/gcse_status")]
        [SwaggerOperation(
            Summary = "Retrieves the list of candidate CGSE status.",
            OperationId = "GetCandidateGcseStatus",
            Tags = new[] { "Types" })]
        [ProducesResponseType(typeof(IEnumerable<TypeEntity>), 200)]
        public async Task<IActionResult> GetCandidateGcseStatus()
        {
            return Ok(await _store.GetPickListItems("contact", "dfe_hasgcseenglish").ToListAsync());
        }

        [HttpGet]
        [CrmETag]
        [Route("candidate/retake_gcse_status")]
        [SwaggerOperation(
            Summary = "Retrieves the list of candidate retake CGSE status.",
            OperationId = "GetCandidateRetakeGcseStatus",
            Tags = new[] { "Types" })]
        [ProducesResponseType(typeof(IEnumerable<TypeEntity>), 200)]
        public async Task<IActionResult> GetCandidateRetakeGcseStatus()
        {
            return Ok(await _store.GetPickListItems("contact", "dfe_websiteplanningretakeenglishgcse").ToListAsync());
        }

        [HttpGet]
        [CrmETag]
        [Route("candidate/describe_yourself")]
        [SwaggerOperation(
            Summary = "Retrieves the list of candidate describe yourself options.",
            OperationId = "GetCandidateDescribeYourselfOptions",
            Tags = new[] { "Types" })]
        [ProducesResponseType(typeof(IEnumerable<TypeEntity>), 200)]
        public async Task<IActionResult> GetCandidateDescribeYourselfOptions()
        {
            return Ok(await _store.GetPickListItems("contact", "dfe_websitedescribeyourself").ToListAsync());
        }

        [HttpGet]
        [CrmETag]
        [Route("candidate/consideration_journey_stages")]
        [SwaggerOperation(
            Summary = "Retrieves the list of candidate consideration journey stages.",
            OperationId = "GetCandidateDescribeYourself",
            Tags = new[] { "Types" })]
        [ProducesResponseType(typeof(IEnumerable<TypeEntity>), 200)]
        public async Task<IActionResult> GetCandidateConsiderationJourneyStages()
        {
            return Ok(await _store.GetPickListItems("contact", "dfe_websitewhereinconsiderationjourney").ToListAsync());
        }

        [HttpGet]
        [CrmETag]
        [Route("candidate/adviser_eligibilities")]
        [SwaggerOperation(
            Summary = "Retrieves the list of candidate adviser eligibilities.",
            OperationId = "GetCandidateAdviserEligibilities",
            Tags = new[] { "Types" })]
        [ProducesResponseType(typeof(IEnumerable<TypeEntity>), 200)]
        public async Task<IActionResult> GetCandidateAdviserEligibilities()
        {
            return Ok(await _store.GetPickListItems("contact", "dfe_iscandidateeligibleforadviser").ToListAsync());
        }

        [HttpGet]
        [CrmETag]
        [Route("candidate/adviser_requirements")]
        [SwaggerOperation(
            Summary = "Retrieves the list of candidate adviser requirements.",
            OperationId = "GetCandidateAdviserRequirements",
            Tags = new[] { "Types" })]
        [ProducesResponseType(typeof(IEnumerable<TypeEntity>), 200)]
        public async Task<IActionResult> GetCandidateAdviserRequirements()
        {
            return Ok(await _store.GetPickListItems("contact", "dfe_isadvisorrequiredos").ToListAsync());
        }

        [HttpGet]
        [CrmETag]
        [Route("candidate/types")]
        [SwaggerOperation(
            Summary = "Retrieves the list of candidate types.",
            OperationId = "GetCandidateTypes",
            Tags = new[] { "Types" })]
        [ProducesResponseType(typeof(IEnumerable<TypeEntity>), 200)]
        public async Task<IActionResult> GetCandidateTypes()
        {
            return Ok(await _store.GetPickListItems("contact", "dfe_typeofcandidate").ToListAsync());
        }

        [HttpGet]
        [CrmETag]
        [Route("candidate/status")]
        [SwaggerOperation(
            Summary = "Retrieves the list of candidate status.",
            OperationId = "GetCandidateStatus",
            Tags = new[] { "Types" })]
        [ProducesResponseType(typeof(IEnumerable<TypeEntity>), 200)]
        public async Task<IActionResult> GetCandidateStatus()
        {
            return Ok(await _store.GetPickListItems("contact", "dfe_candidatestatus").ToListAsync());
        }

        [HttpGet]
        [CrmETag]
        [Route("qualification/degree_status")]
        [SwaggerOperation(
            Summary = "Retrieves the list of qualification degree status.",
            OperationId = "GetQualificationDegreeStatus",
            Tags = new[] { "Types" })]
        [ProducesResponseType(typeof(IEnumerable<TypeEntity>), 200)]
        public async Task<IActionResult> GetQualificationDegreeStatus()
        {
            return Ok(await _store.GetPickListItems("dfe_candidatequalification", "dfe_degreestatus").ToListAsync());
        }

        [HttpGet]
        [CrmETag]
        [Route("qualification/types")]
        [SwaggerOperation(
            Summary = "Retrieves the list of qualification types.",
            OperationId = "GetQualificationTypes",
            Tags = new[] { "Types" })]
        [ProducesResponseType(typeof(IEnumerable<TypeEntity>), 200)]
        public async Task<IActionResult> GetQualificationTypes()
        {
            return Ok(await _store.GetPickListItems("dfe_candidatequalification", "dfe_type").ToListAsync());
        }

        [HttpGet]
        [CrmETag]
        [Route("qualification/uk_degree_grades")]
        [SwaggerOperation(
            Summary = "Retrieves the list of qualification UK degree grades.",
            OperationId = "GetQualificationUkDegreeGrades",
            Tags = new[] { "Types" })]
        [ProducesResponseType(typeof(IEnumerable<TypeEntity>), 200)]
        public async Task<IActionResult> GetQualificationUkDegreeGrades()
        {
            return Ok(await _store.GetPickListItems("dfe_candidatequalification", "dfe_ukdegreegrade").ToListAsync());
        }

        [HttpGet]
        [CrmETag]
        [Route("past_teaching_position/education_phases")]
        [SwaggerOperation(
            Summary = "Retrieves the list of past teaching position education phases.",
            OperationId = "GetPastTeachingPositionEducationPhases",
            Tags = new[] { "Types" })]
        [ProducesResponseType(typeof(IEnumerable<TypeEntity>), 200)]
        public async Task<IActionResult> GetPastTeachingPositionEducationPhases()
        {
            return Ok(await _store.GetPickListItems("dfe_candidatepastteachingposition", "dfe_educationphase").ToListAsync());
        }

        [HttpGet]
        [CrmETag]
        [Route("teaching_event/types")]
        [SwaggerOperation(
            Summary = "Retrieves the list of teaching event types.",
            OperationId = "GetTeachingEventTypes",
            Tags = new[] { "Types" })]
        [ProducesResponseType(typeof(IEnumerable<TypeEntity>), 200)]
        public async Task<IActionResult> GetTeachingEventTypes()
        {
            return Ok(await _store.GetPickListItems("msevtmgt_event", "dfe_event_type").ToListAsync());
        }

        [HttpGet]
        [CrmETag]
        [Route("teaching_event_registration/channels")]
        [SwaggerOperation(
            Summary = "Retrieves the list of teaching event registration channels.",
            OperationId = "GetTeachingEventRegistrationChannels",
            Tags = new[] { "Types" })]
        [ProducesResponseType(typeof(IEnumerable<TypeEntity>), 200)]
        public async Task<IActionResult> GetTeachingEventRegistrationChannels()
        {
            return Ok(await _store.GetPickListItems("msevtmgt_eventregistration", "dfe_channelcreation").ToListAsync());
        }

        [HttpGet]
        [CrmETag]
        [Route("phone_call/channels")]
        [SwaggerOperation(
            Summary = "Retrieves the list of phone call channels.",
            OperationId = "GetPhoneCallChannels",
            Tags = new[] { "Types" })]
        [ProducesResponseType(typeof(IEnumerable<TypeEntity>), 200)]
        public async Task<IActionResult> GetPhoneCallChannels()
        {
            return Ok(await _store.GetPickListItems("phonecall", "dfe_channelcreation").ToListAsync());
        }

        [HttpGet]
        [CrmETag]
        [Route("phone_call/destinations")]
        [SwaggerOperation(
            Summary = "Retrieves the list of phone call destinations.",
            OperationId = "GetPhoneCallDestinations",
            Tags = new[] { "Types" })]
        [ProducesResponseType(typeof(IEnumerable<TypeEntity>), 200)]
        public async Task<IActionResult> GetPhoneCallDestinations()
        {
            return Ok(await _store.GetPickListItems("phonecall", "dfe_destination").ToListAsync());
        }

        [HttpGet]
        [CrmETag]
        [Route("service_subscription/types")]
        [SwaggerOperation(
            Summary = "Retrieves the list of subscription types.",
            OperationId = "GetSubscriptionTypes",
            Tags = new[] { "Types" })]
        [ProducesResponseType(typeof(IEnumerable<TypeEntity>), 200)]
        public async Task<IActionResult> GetSubscriptionTypes()
        {
            return Ok(await _store.GetPickListItems("dfe_servicesubscription", "dfe_servicesubscriptiontype").ToListAsync());
        }
    }
}
