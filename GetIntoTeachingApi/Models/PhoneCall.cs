using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GetIntoTeachingApi.Models
{
    [Table("phonecall")]
    public class PhoneCall : BaseModel
    {
        [Column("phonecallid")]
        public new Guid? Id { get => base.Id; set => base.Id = value; }

        [Column("scheduledstart")]
        public DateTime ScheduledAt { get; set; }
        [Column("phonenumber")]
        public string Telephone { get; set; }

        public PhoneCall() : base() { }
    }
}
