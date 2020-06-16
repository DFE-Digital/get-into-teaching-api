using System.Collections.Generic;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Controllers
{
    [Route("api/candidates")]
    [ApiController]
    [Authorize]
    public class CandidatesController : ControllerBase
    {
        private readonly ICandidateAccessTokenService _tokenService;
        private readonly INotifyService _notifyService;
        private readonly ICrmService _crm;

        public CandidatesController(
            ICandidateAccessTokenService tokenService,
            INotifyService notifyService,
            ICrmService crm)
        {
            _tokenService = tokenService;
            _notifyService = notifyService;
            _crm = crm;
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

            var candidate = _crm.GetCandidate(request);
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
    }
}
