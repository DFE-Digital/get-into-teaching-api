using System.Collections.Generic;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Controllers
{
    [Route("api/mailing_list")]
    [ApiController]
    [Authorize]
    public class MailingListController : ControllerBase
    {
        private readonly ICandidateAccessTokenService _tokenService;
        private readonly ICrmService _crm;
        private readonly IBackgroundJobClient _jobClient;

        public MailingListController(
            ICandidateAccessTokenService tokenService,
            ICrmService crm,
            IBackgroundJobClient jobClient)
        {
            _crm = crm;
            _tokenService = tokenService;
            _jobClient = jobClient;
        }

        [HttpPost]
        [Route("members")]
        [SwaggerOperation(
            Summary = "Adds a new member to the mailing list.",
            Description = "If the `CandidateId` is specified then the existing candidate will be " +
                          "added to the mailing list, otherwise a new candidate will be created.",
            OperationId = "AddMailingListMember",
            Tags = new[] { "Mailing List" })]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        public IActionResult AddMember(
            [FromBody, SwaggerRequestBody("Member to add to the mailing list.", Required = true)] MailingListAddMember request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }

            _jobClient.Enqueue<UpsertCandidateJob>((x) => x.Run(request.Candidate, null));

            return NoContent();
        }

        [HttpPost]
        [Route("members/{accessToken}")]
        [SwaggerOperation(
            Summary = "Retrieves a pre-populated MailingListAddMember for the candidate.",
            Description = @"
Retrieves a pre-populated MailingListAddMember for the candidate. The `accessToken` is obtained from a 
`POST /candidates/access_tokens` request (you must also ensure the `ExistingCandidateRequest` payload you 
exchanged for your token matches the request payload here).",
            OperationId = "GetPreFilledMailingListAddMember",
            Tags = new[] { "Mailing List" })]
        [ProducesResponseType(typeof(MailingListAddMember), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetMember(
            [FromRoute, SwaggerParameter("Access token (PIN code).", Required = true)] string accessToken,
            [FromBody, SwaggerRequestBody("Candidate access token request (must match an existing candidate).", Required = true)] ExistingCandidateRequest request)
        {
            if (!_tokenService.IsValid(accessToken, request))
            {
                return Unauthorized();
            }

            var candidate = _crm.MatchCandidate(request);

            if (candidate == null)
            {
                return NotFound();
            }

            return Ok(new MailingListAddMember(candidate));
        }
    }
}