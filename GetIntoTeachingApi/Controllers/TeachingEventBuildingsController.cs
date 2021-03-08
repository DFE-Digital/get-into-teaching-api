using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Controllers
{
    [Route("api/teaching_event_buildings")]
    [ApiController]
    [Authorize(Roles = "Admin,GetIntoTeaching")]
    public class TeachingEventBuildingsController : ControllerBase
    {
        private readonly IStore _store;

        public TeachingEventBuildingsController(IStore store)
        {
            _store = store;
        }

        [HttpGet]
        [CrmETag]
        [PrivateShortTermResponseCache]
        [Route("teaching_event_buildings")]
        [SwaggerOperation(
            Summary = "Retrieves all event buildings.",
            OperationId = "GetTeachingEventBuildings",
            Tags = new[] { "Teaching Events" })]
        [ProducesResponseType(typeof(TeachingEventBuilding), 200)]
        public IActionResult GetTeachingEventBuildings()
        {
            var buildings = _store.GetTeachingEventBuildings();

            return Ok(buildings);
        }
    }
}
