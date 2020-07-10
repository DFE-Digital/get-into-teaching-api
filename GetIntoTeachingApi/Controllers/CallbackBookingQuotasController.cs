using System.Collections.Generic;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Controllers
{
    [Route("api/callback_booking_quotas")]
    [ApiController]
    [Authorize]
    public class CallbackBookingQuotasController : ControllerBase
    {
        private readonly ICrmService _crm;

        public CallbackBookingQuotasController(ICrmService crm)
        {
            _crm = crm;
        }

        [HttpGet]
        [Route("")]
        [SwaggerOperation(
            Summary = "Retrieves all callback booking quotas.",
            OperationId = "GetCallbackBookingQuotas",
            Tags = new[] { "Callback Booking Quotas" })]
        [ProducesResponseType(typeof(IEnumerable<CallbackBookingQuota>), 200)]
        public IActionResult GetAll()
        {
            var quotas = _crm.GetCallbackBookingQuotas();
            return Ok(quotas);
        }
    }
}