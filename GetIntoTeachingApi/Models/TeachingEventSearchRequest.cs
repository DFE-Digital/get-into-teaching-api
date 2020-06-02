using System;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Models
{
    public class TeachingEventSearchRequest
    {
        [SwaggerSchema("Postcode to center search around.")]
        public string Postcode { get; set; }
        [SwaggerSchema("Set to filter results to a radius (in miles) around the postcode.")]
        public int? Radius { get; set; }
        [SwaggerSchema("Set to filter results to a type of teaching event. Must match an `typeId` of the `TeachingEvent` schema.")]
        public int? TypeId { get; set; }
        [SwaggerSchema("Set to filter results to those that start after a given date.")]
        public DateTime? StartAfter { get; set; }
        [SwaggerSchema("Set to filter results to those that start before a given date.")]
        public DateTime? StartBefore { get; set; }
    }
}
