using GetIntoTeachingApi.Models.Crm;
using System.Collections.Generic;

namespace GetIntoTeachingApi.Models.GetIntoTeaching
{
    public class TeachingEventsByType
    {
        public int TypeId { get; set; }
        public IEnumerable<TeachingEvent> TeachingEvents { get; set; }
    }
}
