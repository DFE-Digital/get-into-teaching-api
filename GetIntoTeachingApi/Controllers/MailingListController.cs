﻿using System;
using System.Collections.Generic;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Controllers
{
    [Route("api/mailing_list")]
    [ApiController]
    [Authorize(Roles = "Admin,GetIntoTeaching")]
    public class MailingListController : ControllerBase
    {
        private readonly ICandidateAccessTokenService _accessTokenService;
        private readonly ICandidateMagicLinkTokenService _magicLinkTokenService;
        private readonly ICrmService _crm;
        private readonly IBackgroundJobClient _jobClient;

        public MailingListController(
            ICandidateAccessTokenService accessTokenService,
            ICandidateMagicLinkTokenService magicLinkTokenService,
            ICrmService crm,
            IBackgroundJobClient jobClient)
        {
            _crm = crm;
            _accessTokenService = accessTokenService;
            _magicLinkTokenService = magicLinkTokenService;
            _jobClient = jobClient;
        }

        [HttpPost]
        [Route("members")]
        [SwaggerOperation(
            Summary = "Adds a new member to the mailing list.",
            Description = "If the `CandidateId` is specified then the existing candidate will be " +
                          "added to the mailing list, otherwise a new candidate will be created." +
                          "\n\n" +
                          "Validation errors may be present on the `MailingListAddMember` object as " +
                          "well as the hidden `Candidate` model that is mapped to; property names are " +
                          "consistent, so you should check for inclusion of the field in the key " +
                          "when linking an error message back to a property on the request model. For " +
                          "example, an error on `UkDegreeGradeId` can return under the keys " +
                          "`Candidate.Qualifications[0].UkDegreeGradeId` and `UkDegreeGradeId`.",
            OperationId = "AddMailingListMember",
            Tags = new[] { "Mailing List" })]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status400BadRequest)]
        public IActionResult AddMember(
            [FromBody, SwaggerRequestBody("Member to add to the mailing list.", Required = true)] MailingListAddMember request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }

            string json = request.Candidate.SerializeChangeTracked();
            _jobClient.Enqueue<UpsertCandidateJob>((x) => x.Run(json, null));

            return NoContent();
        }

        [HttpPost]
        [Route("members/exchange_access_token/{accessToken}")]
        [SwaggerOperation(
            Summary = "Retrieves a pre-populated MailingListAddMember for the candidate.",
            Description = @"
                Retrieves a pre-populated MailingListAddMember for the candidate. The `accessToken` is obtained from a 
                `POST /candidates/access_tokens` request (you must also ensure the `ExistingCandidateRequest` payload you 
                exchanged for your token matches the request payload here).",
            OperationId = "ExchangeAccessTokenForMailingListAddMember",
            Tags = new[] { "Mailing List" })]
        [ProducesResponseType(typeof(MailingListAddMember), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult ExchangeAccessTokenForMember(
            [FromRoute, SwaggerParameter("Access token (PIN code).", Required = true)] string accessToken,
            [FromBody, SwaggerRequestBody("Candidate access token request (must match an existing candidate).", Required = true)] ExistingCandidateRequest request)
        {
            var candidate = _crm.MatchCandidate(request);

            if (candidate == null || !_accessTokenService.IsValid(accessToken, request, (Guid)candidate.Id))
            {
                return Unauthorized();
            }

            return Ok(new MailingListAddMember(candidate));
        }

        [HttpGet]
        [Route("members/exchange_magic_link_token/{magicLinkToken}")]
        [SwaggerOperation(
            Summary = "Retrieves a pre-populated MailingListAddMember for the candidate.",
            Description = @"
                Retrieves a pre-populated MailingListAddMember for the candidate. The `magicLinkToken` is obtained from a 
                `POST /candidates/magic_link_tokens` request.",
            OperationId = "ExchangeMagicLinkTokenForMailingListAddMember",
            Tags = new[] { "Mailing List" })]
        [ProducesResponseType(typeof(MailingListAddMember), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CandidateMagicLinkExchangeResult), StatusCodes.Status401Unauthorized)]
        public IActionResult ExchangeMagicLinkTokenForMember(
            [FromRoute, SwaggerParameter("Magic link token.", Required = true)] string magicLinkToken)
        {
            var result = _magicLinkTokenService.Exchange(magicLinkToken);

            if (!result.Success)
            {
                return Unauthorized(result);
            }

            string json = result.Candidate.SerializeChangeTracked();
            _jobClient.Enqueue<UpsertCandidateJob>((x) => x.Run(json, null));

            return Ok(new MailingListAddMember(result.Candidate));
        }
    }
}