using GetIntoTeaching.Core.CrossCuttingConcerns.Mediator;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models.TeacherTrainingAdviser;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace GetIntoTeachingApi.Controllers.Spencer
{
    public class SpencersCandidatesController : ControllerBase
    {
        private readonly IBackgroundJobClient _jobClient;
        private readonly IDateTimeProvider _dateTime;
        private readonly ILogger<CandidatesController> _logger;

        public SpencersCandidatesController(
            IBackgroundJobClient jobClient,
            IDateTimeProvider dateTime,
            ILogger<CandidatesController> logger,

            IMediator mediator)
        {
            _jobClient = jobClient;
            _dateTime = dateTime;
            _logger = logger;
        }

        [HttpPost("TESTER/")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status400BadRequest)]
        public IActionResult SignUp([FromBody] TeacherTrainingAdviserSignUp request)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(this.ModelState);
            //}

            // This is the only way we can mock/freeze the current date/time
            // in contract tests (there's no other way to inject it into this class).
            request.DateTimeProvider = _dateTime;
            string json = request.Candidate.SerializeChangeTracked();
            _jobClient.Enqueue<UpsertCandidateJob>((x) => x.Run(json, null));

            _logger.LogInformation("TeacherTrainingAdviser - CandidatesController - Sign Up - {Client}", User.Identity.Name);

            return NoContent();
        }
    }
}
