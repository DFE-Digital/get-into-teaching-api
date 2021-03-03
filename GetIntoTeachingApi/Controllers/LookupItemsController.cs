using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Controllers
{
    [Route("api/lookup_items")]
    [ApiController]
    [PrivateShortTermResponseCache]
    [Authorize]
    public class LookupItemsController : ControllerBase
    {
        private readonly IStore _store;

        public LookupItemsController(IStore store)
        {
            _store = store;
        }

        [HttpGet]
        [CrmETag]
        [Route("countries")]
        [SwaggerOperation(
            Summary = "Retrieves the list of countries.",
            OperationId = "GetCountries",
            Tags = new[] { "Lookup Items" })]
        [ProducesResponseType(typeof(IEnumerable<LookupItem>), 200)]
        public async Task<IActionResult> GetCountries()
        {
            var countries = await _store.GetLookupItems("dfe_country").ToListAsync();

            return Ok(countries.OrderBy(c => c.Value));
        }

        [HttpGet]
        [CrmETag]
        [Route("teaching_subjects")]
        [SwaggerOperation(
            Summary = "Retrieves the list of teaching subjects.",
            OperationId = "GetTeachingSubjects",
            Tags = new[] { "Lookup Items" })]
        [ProducesResponseType(typeof(IEnumerable<LookupItem>), 200)]
        public async Task<IActionResult> GetTeachingSubjects()
        {
            var subjects = await _store.GetLookupItems("dfe_teachingsubjectlist").ToListAsync();

            return Ok(subjects.OrderBy(c => c.Value));
        }
    }
}
