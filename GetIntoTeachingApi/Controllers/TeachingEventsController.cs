using System;
using System.Collections.Generic;
using System.Linq;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Controllers
{
    [Route("api/teaching_events")]
    [ApiController]
    [Authorize(Policy = "SharedSecret")]
    public class TeachingEventsController : ControllerBase
    {
        private const int MaximumUpcomingRequests = 50;
        private readonly ILogger<TeachingEventsController> _logger;
        private readonly ICrmService _crm;

        public TeachingEventsController(ILogger<TeachingEventsController> logger, ICrmService crm)
        {
            _logger = logger;
            _crm = crm;
        }

        [HttpGet]
        [Route("upcoming")]
        [SwaggerOperation(
            Summary = "Retrieves the upcoming teaching events.",
            Description = @"
Retrieves the upcoming teaching events; limited to 10 by default, but this can be increased to a 
maximum of 50 using the `limit` query parameter.",
            OperationId = "GetUpcomingTeachingEvents",
            Tags = new[] { "Teaching Events" }
        )]
        [ProducesResponseType(typeof(IEnumerable<TeachingEvent>), 200)]
        [ProducesResponseType(400)]
        public IActionResult GetUpcoming([FromQuery, SwaggerParameter("Number of results to return (maximum of 50).")] int limit = 10)
        {
            if (limit > MaximumUpcomingRequests)
                return BadRequest();

            var upcomingEvents = _crm.GetUpcomingTeachingEvents(limit);
            return Ok(upcomingEvents);
        }

        [HttpGet]
        [Route("search")]
        [SwaggerOperation(
            Summary = "Searches for teaching events.",
            Description = @"Searches for teaching events by postcode. Optionally limit the results by distance (in miles) and the type of event.",
            OperationId = "SearchTeachingEvents",
            Tags = new[] { "Teaching Events" }
        )]
        [ProducesResponseType(typeof(IEnumerable<TeachingEvent>), 200)]
        [ProducesResponseType(400)]
        public IActionResult Search([FromQuery, SwaggerParameter("Event search criteria.", Required = true)] TeachingEventSearchRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(this.ModelState);
            }

            var teachingEvents = _crm.SearchTeachingEvents(request);
            return Ok(teachingEvents);
        }

        [HttpGet]
        [Route("{readableEventId}")]
        [SwaggerOperation(
            Summary = "Retrieves an event.",
            OperationId = "GetTeachingEvent",
            Tags = new[] { "Teaching Events" }
        )]
        public IActionResult Get([FromRoute, SwaggerParameter("The `readableEventId` of the `TeachingEvent`.", Required = true)] string readableEventId)
        {
            // TODO:
            return Ok(new Object());
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Creates a new teaching event.",
            OperationId = "CreateTeachingEvent",
            Tags = new[] { "Teaching Events" }
        )]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        public IActionResult Create([FromBody, SwaggerRequestBody("Teaching event to create.", Required = true)] Object teachingEvent) // TODO:
        {
            // TODO:
            return Ok(new Object());
        }

        [HttpPost]
        [Route("{readableEventId}/attendees")]
        [SwaggerOperation(
            Summary = "Adds an attendee to a teaching event.",
            OperationId = "AddTeachingEventAttendee",
            Tags = new[] { "Teaching Events" }
        )]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        public IActionResult AddAttendee(
            [FromRoute, SwaggerParameter("The `readableEventId` of the `TeachingEvent`.", Required = true)] string readableEventId,
            [FromBody, SwaggerRequestBody("Attendee to add to the teaching event.", Required = true)] CandidateIdentification attendee
        )
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
