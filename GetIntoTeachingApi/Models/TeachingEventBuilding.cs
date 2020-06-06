using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;
using NetTopologySuite.Geometries;

namespace GetIntoTeachingApi.Models
{
    [Entity(LogicalName = "msevtmgt_building")]
    public class TeachingEventBuilding : BaseModel
    {
        [EntityField(Name = "msevtmgt_addressline1")]
        public string AddressLine1 { get; set; }
        [EntityField(Name = "msevtmgt_addressline2")]
        public string AddressLine2 { get; set; }
        [EntityField(Name = "msevtmgt_addressline3")]
        public string AddressLine3 { get; set; }
        [EntityField(Name = "msevtmgt_city")]
        public string AddressCity { get; set; }
        [EntityField(Name = "msevtmgt_stateprovince")]
        public string AddressState { get; set; }
        [EntityField(Name = "msevtmgt_postalcode")]
        public string AddressPostcode { get; set; }
        [JsonIgnore]
        [Column(TypeName = "geography")]
        public Point Coordinate { get; set; }

        public TeachingEventBuilding() : base() { }

        public TeachingEventBuilding(Entity entity, ICrmService crm) : base(entity, crm) { }
    }
}
