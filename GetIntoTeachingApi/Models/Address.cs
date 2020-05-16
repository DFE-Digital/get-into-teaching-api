using GetIntoTeachingApi.Attributes;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models
{
    public class Address : BaseModel
    {
        [Entity(Name = "address1_line1")]
        public string Line1 { get; set; }
        [Entity(Name = "address1_line2")]
        public string Line2 { get; set; }
        [Entity(Name = "address1_line3")]
        public string Line3 { get; set; }
        [Entity(Name = "address1_city")]
        public string City { get; set; }
        [Entity(Name = "address1_stateorprovince")]
        public string State { get; set; }
        [Entity(Name = "address1_postalcode")]
        public string Postcode { get; set; }

        public Address() : base() { }

        public Address(Entity entity) : base(entity) { }
    }
}
