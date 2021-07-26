using GetIntoTeachingApi.Models.Crm;
using System;

namespace GetIntoTeachingApi.Models.GetIntoTeaching
{
    public class TeachingEventUpsertOperation
    {
        public Guid? Id { get; set; }
        public string ReadableId { get; set; }

        public TeachingEventUpsertOperation(TeachingEvent teachingEvent)
        {
            Id = teachingEvent.Id;
            ReadableId = teachingEvent.ReadableId;
        }

        public TeachingEventUpsertOperation()
        {
        }
    }
}
