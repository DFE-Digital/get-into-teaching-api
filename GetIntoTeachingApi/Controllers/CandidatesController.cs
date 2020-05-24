using GetIntoTeachingApi.Models;
using Microsoft.AspNetCore.Authorization;
using GetIntoTeachingApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using GetIntoTeachingApi.Services.Crm;

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
        private readonly IWebApiClient _client;

        public CandidatesController(
            ILogger<CandidatesController> logger,
            ICandidateAccessTokenService tokenService,
            INotifyService notifyService,
            IWebApiClient client
        )
        {
            _logger = logger;
            _tokenService = tokenService;
            _notifyService = notifyService;
            _client = client;
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
            Tags = new[] { "Candidates" }
        )]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        public async Task<IActionResult> CreateAccessToken([FromBody, SwaggerRequestBody("Candidate access token request (must match an existing candidate).", Required = true)] ExistingCandidateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(this.ModelState);

            var candidate = await _client.GetCandidate(request);
            if (candidate == null)
                return NotFound();

            var token = _tokenService.GenerateToken(request);
            var personalisation = new Dictionary<string, dynamic> { { "pin_code", token } };
#pragma warning disable 4014
            // Don't wait for a response; respond 204 immediately.
            _notifyService.SendEmail(request.Email, NotifyService.NewPinCodeEmailTemplateId, personalisation);
#pragma warning restore 4014

            return NoContent();
        }
    }
}
