using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using FluentValidation;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;
using NetTopologySuite.Geometries;

namespace GetIntoTeachingApi.Models
{
    [Entity("msevtmgt_building")]
    public class TeachingEventBuilding : BaseModel
    {
        [EntityField("msevtmgt_name")]
        public string Venue { get; set; }
        [EntityField("msevtmgt_addressline1")]
        public string AddressLine1 { get; set; }
        [EntityField("msevtmgt_addressline2")]
        public string AddressLine2 { get; set; }
        [EntityField("msevtmgt_addressline3")]
        public string AddressLine3 { get; set; }
        [EntityField("msevtmgt_city")]
        public string AddressCity { get; set; }
        [EntityField("msevtmgt_postalcode")]
        public string AddressPostcode { get; set; }
        [EntityField("dfe_eventvenueimageurl")]
        public string ImageUrl { get; set; }
        [JsonIgnore]
        [Column(TypeName = "geography")]
        public Point Coordinate { get; set; }
        [JsonIgnore]
        public IEnumerable<TeachingEvent> TeachingEvents { get; set; }

        public TeachingEventBuilding()
            : base()
        {
        }

        public TeachingEventBuilding(Entity entity, ICrmService crm, IValidatorFactory validatorFactory)
            : base(entity, crm, validatorFactory)
        {
        }
    }
}
