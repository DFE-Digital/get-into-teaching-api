using System;
using System.Collections.Generic;
using GetIntoTeachingApi.Models;
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
        [HttpPost]
        [Route("members")]
        [SwaggerOperation(
            Summary = "Adds a new member to the mailing list.",
            Description = "Adds a new member to the mailing list. A new candidate will also be created if a matching candidate can not be found.",
            OperationId = "AddMailingListMember",
            Tags = new[] { "Mailing List" })]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        public IActionResult AddMember([FromBody, SwaggerRequestBody("Member to add to the mailing list.", Required = true)] ExistingCandidateRequest member)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }

            // TODO:
            return Ok(new Object());
        }
    }
}