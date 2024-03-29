﻿using System.Collections.Generic;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Controllers.GetIntoTeaching
{
    [Route("api/teaching_event_buildings")]
    [ApiController]
    [Authorize(Roles = "Admin,GetIntoTeaching")]
    [PrivateShortTermResponseCache]
    public class TeachingEventBuildingsController : ControllerBase
    {
        private readonly IStore _store;

        public TeachingEventBuildingsController(IStore store)
        {
            _store = store;
        }

        [HttpGet]
        [Route("")]
        [SwaggerOperation(
            Summary = "Retrieves all event buildings.",
            OperationId = "GetTeachingEventBuildings",
            Tags = new[] { "Teaching Event Buildings" })]
        [ProducesResponseType(typeof(IEnumerable<TeachingEventBuilding>), 200)]
        public IActionResult GetTeachingEventBuildings()
        {
            var buildings = _store.GetTeachingEventBuildings();

            return Ok(buildings);
        }
    }
}
