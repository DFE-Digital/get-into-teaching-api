using System;
using System.Collections.Generic;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Controllers
{
    [Route("api/candidates")]
    [ApiController]
    [LogRequests]
    [Authorize]
    public class CandidatesController : ControllerBase
    {
        private readonly ICandidateAccessTokenService _tokenService;
        private readonly INotifyService _notifyService;
        private readonly ICrmService _crm;
        private readonly IBackgroundJobClient _jobClient;

        public CandidatesController(
            ICandidateAccessTokenService tokenService,
            INotifyService notifyService,
            ICrmService crm,
            IBackgroundJobClient jobClient)
        {
            _tokenService = tokenService;
            _notifyService = notifyService;
            _crm = crm;
            _jobClient = jobClient;
        }

        [HttpPost]
        [Route("access_tokens")]
        [SwaggerOperation(
            Summary = "Creates a candidate access token.",
            Description = @"
Finds a candidate matching at least 3 of the provided CandidateAccessTokenRequest attributes (including email). 
If a candidate is found, an access token (PIN code) will be sent to the candidate email address 
that can then be used for verification.",
            OperationId = "CreateCandidateAccessToken",
            Tags = new[] { "Candidates" })]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        public IActionResult CreateAccessToken([FromBody, SwaggerRequestBody("Candidate access token request (must match an existing candidate).", Required = true)] ExistingCandidateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }

            var candidate = _crm.MatchCandidate(request);
            if (candidate == null)
            {
                return NotFound();
            }

            var token = _tokenService.GenerateToken(request);
            var personalisation = new Dictionary<string, dynamic> { { "pin_code", token } };

            // We respond immediately/assume this will be successful.
            _notifyService.SendEmailAsync(request.Email, NotifyService.NewPinCodeEmailTemplateId, personalisation);

            return NoContent();
        }

        [HttpPost]
        [Route("{candidateId}/long_lived_access_tokens")]
        [SwaggerOperation(
    Summary = "Creates a long-lived candidate access token.",
    Description = @"Creates a long-lived access token that can be exchanged for candidate information for up to 3 days.
        Access tokens are stored against the contact in Dynamics CRM and are issued to candidates by email.",
    OperationId = "CreateLongLivedCandidateAccessToken",
    Tags = new[] { "Candidates" })]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult CreateLongLivedAccessToken([FromRoute, SwaggerRequestBody("Candidate identifier.", Required = true)] Guid candidateId)
        {
            var candidate = _crm.GetCandidate(candidateId);

            if (candidate == null)
            {
                return NotFound();
            }

            var token = Guid.NewGuid().ToString();
            candidate.LongLivedAccessToken = token;

            _jobClient.Enqueue<UpsertCandidateJob>((x) => x.Run(candidate, null));

            return NoContent();
        }
    }
}
