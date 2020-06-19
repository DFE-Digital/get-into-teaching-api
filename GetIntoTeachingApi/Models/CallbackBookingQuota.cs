using System;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    [Entity(LogicalName = "dfe_callbackbookingquota")]
    public class CallbackBookingQuota : BaseModel
    {
        [EntityField(Name = "dfe_name")]
        public string TimeSlot { get; set; }
        [EntityField(Name = "dfe_workingdayname")]
        public string Day { get; set; }
        [EntityField(Name = "dfe_starttime")]
        public DateTime StartAt { get; set; }
        [EntityField(Name = "dfe_endtime")]
        public DateTime EndAt { get; set; }
        [EntityField(Name = "dfe_numberofbookings")]
        public int NumberOfBookings { get; set; }
        [EntityField(Name = "dfe_quota")]
        public int Quota { get; set; }

        public bool IsAvailable => NumberOfBookings < Quota;

        public CallbackBookingQuota()
            : base()
        {
        }

        public CallbackBookingQuota(Entity entity, ICrmService crm)
            : base(entity, crm)
        {
        }
    }
}
