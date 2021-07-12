using System.Collections.Generic;
using GetIntoTeachingApi.Models.Crm;

namespace GetIntoTeachingApi.Models
{
    public class TeachingEventsByType
    {
        public int TypeId { get; set; }
        public IEnumerable<TeachingEvent> TeachingEvents { get; set; }
    }
}
