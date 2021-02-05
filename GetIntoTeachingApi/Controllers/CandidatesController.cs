using System;
using System.Collections.Generic;
using System.Linq;
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
    [Authorize(Roles = "Admin,GetIntoTeaching,GetAnAdviser")]
    public class CandidatesController : ControllerBase
    {
        private const int MaximumMagicLinkTokensPerBatch = 25;

        private readonly ICandidateAccessTokenService _accessTokenService;
        private readonly ICandidateMagicLinkTokenService _magicLinkTokenService;
        private readonly INotifyService _notifyService;
        private readonly ICrmService _crm;
        private readonly IBackgroundJobClient _jobClient;

        public CandidatesController(
            ICandidateAccessTokenService accessTokenService,
            ICandidateMagicLinkTokenService magicLinkTokenService,
            INotifyService notifyService,
            ICrmService crm,
            IBackgroundJobClient jobClient)
        {
            _accessTokenService = accessTokenService;
            _magicLinkTokenService = magicLinkTokenService;
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

            var token = _accessTokenService.GenerateToken(request, (Guid)candidate.Id);
            var personalisation = new Dictionary<string, dynamic> { { "pin_code", token } };

            // We respond immediately/assume this will be successful.
            _notifyService.SendEmailAsync(request.Email, NotifyService.NewPinCodeEmailTemplateId, personalisation);

            return NoContent();
        }

        [HttpPost]
        [Route("magic_link_tokens")]
        [SwaggerOperation(
            Summary = "Creates a token for use in a magic link.",
            Description = @"Creates a long-lived magic link token that can be exchanged for candidate information for up to 48 hours.
                Magic link tokens are stored against the contact in Dynamics CRM and are issued to candidates by email.",
            OperationId = "CreateCandidateMagicLinkToken",
            Tags = new[] { "Candidates" })]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateMagicLinkToken([FromBody, SwaggerRequestBody("Candidate identifiers.", Required = true)] IEnumerable<Guid> candidateIds)
        {
            if (candidateIds.Count() > MaximumMagicLinkTokensPerBatch)
            {
                return BadRequest($"You can only generate {MaximumMagicLinkTokensPerBatch} magic link tokens per request.");
            }

            var candidates = _crm.GetCandidates(candidateIds);

            if (candidates.Count() != candidateIds.Count())
            {
                var missingCandidateIds = candidateIds.Except(candidates.Select(c => (Guid)c.Id));

                return BadRequest(new { message = "Candidate IDs could not be found.", missingCandidateIds });
            }

            foreach (var candidate in candidates)
            {
                _magicLinkTokenService.GenerateToken(candidate);
                _jobClient.Enqueue<UpsertCandidateJob>(x => x.Run(candidate, null));
            }

            return NoContent();
        }
    }
}
