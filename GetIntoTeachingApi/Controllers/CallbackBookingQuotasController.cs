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
    [Authorize(Roles = "Admin,GetAnAdviser")]
    public class CallbackBookingQuotasController : ControllerBase
    {
        private readonly ICallbackBookingService _callbackBookingService;

        public CallbackBookingQuotasController(ICallbackBookingService callbackBookingService)
        {
            _callbackBookingService = callbackBookingService;
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
            var quotas = _callbackBookingService.GetCallbackBookingQuotas();
            return Ok(quotas);
        }
    }
}