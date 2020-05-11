using GetIntoTeachingApi.Models;
using Microsoft.AspNetCore.Authorization;
using GetIntoTeachingApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetIntoTeachingApi.Controllers
{
    [Route("api/candidates")]
    [ApiController]
    [Authorize(Policy = "SharedSecret")]
    public class CandidatesController : ControllerBase
    {
        private readonly ILogger<CandidatesController> _logger;
        private readonly ICandidateAccessTokenService _tokenService;
        private readonly INotifyService _notifyService;

        public CandidatesController(
            ILogger<CandidatesController> logger, 
            ICandidateAccessTokenService tokenService, 
            INotifyService notifyService
        )
        {
            _logger = logger;
            _tokenService = tokenService;
            _notifyService = notifyService;
        }

        [HttpPost]
        [Route("access_tokens")]
        [SwaggerOperation(
            Summary = "Creates a candidate access token.",
            Description = @"
Finds a candidate matching at least 3 of the provided CandidateAccessTokenRequest attributes. 
If a candidate is found, an access token (PIN code) will be sent to the candidate email address 
that can then be used for verification.",
            OperationId = "CreateCandidateAccessToken",
            Tags = new[] { "Candidates" }
        )]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        public IActionResult CreateAccessToken([FromBody, SwaggerRequestBody("Candidate access token request (must match an existing candidate).", Required = true)] CandidateAccessTokenRequest candidate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }

            if (true) // TODO:
            {
                string token = _tokenService.GenerateToken(candidate.Email);
                var personalisation = new Dictionary<string, dynamic> { { "pin_code", token } };
                _notifyService.SendEmail(candidate.Email, NotifyService.NewPinCodeEmailTemplateId, personalisation);
            }

            // TODO:
            return NoContent();
        }
    }
}
