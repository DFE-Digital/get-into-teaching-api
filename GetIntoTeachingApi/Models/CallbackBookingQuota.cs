using System;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    [Entity("dfe_callbackbookingquota")]
    public class CallbackBookingQuota : BaseModel
    {
        [EntityField("dfe_name")]
        public string TimeSlot { get; set; }
        [EntityField("dfe_workingdayname")]
        public string Day { get; set; }
        [EntityField("dfe_starttime")]
        public DateTime StartAt { get; set; }
        [EntityField("dfe_endtime")]
        public DateTime EndAt { get; set; }
        [EntityField("dfe_numberofbookings")]
        public int NumberOfBookings { get; set; }
        [EntityField("dfe_quota")]
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
