﻿using System;
using GetIntoTeachingApi.Models.Crm;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Models.GetIntoTeaching
{
    public class TeachingEventSearchRequest : ICloneable
    {
        [SwaggerSchema("Postcode to center search around.")]
        public string Postcode { get; set; }
        [SwaggerSchema("Set to filter results to a radius (in miles) around the postcode.")]
        public int? Radius { get; set; }
        [SwaggerSchema("Set to filter results to a type of teaching event. Each ID must match a `typeId` of the `TeachingEvent` schema.")]
        public int[] TypeIds { get; set; }
        [SwaggerSchema("Set to true to return only online events and false to return only in-person events.")]
        public bool? Online { get; set; }
        [SwaggerSchema("Set to filter results to those that start after a given date.")]
        public DateTime? StartAfter { get; set; }
        [SwaggerSchema("Set to filter results to those that start before a given date.")]
        public DateTime? StartBefore { get; set; }
        [SwaggerSchema("Set to filter results by event status.")]
        public int[] StatusIds { get; set; } = new int[] { (int)TeachingEvent.Status.Open, (int)TeachingEvent.Status.Closed };
        [SwaggerSchema("Set to filter results by accessibility options.")]
        public int[] AccessibilityOptions { get; set; }

        public double? RadiusInKm() => Radius * 1.60934;

        public object Clone()
        {
            return MemberwiseClone();
        }

        public TeachingEventSearchRequest Clone(Action<TeachingEventSearchRequest> block)
        {
            var clone = (TeachingEventSearchRequest)Clone();
            block.Invoke(clone);
            return clone;
        }
    }
}
