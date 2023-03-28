using System;
using Microsoft.Xrm.Sdk;
using System.ComponentModel.DataAnnotations.Schema;

namespace GetIntoTeachingApi.Models
{
	public class Country
	{
        public static readonly Guid UnitedKingdomCountryId = new("72f5c2e6-74f9-e811-a97a-000d3a2760f2");

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid? Id { get; set; }
        public string Value { get; set; }
        public string IsoCode { get; set; }

        public Country()
        {
        }

        public Country(Entity entity)
        {
            Id = entity.Id;
            Value = entity.GetAttributeValue<string>("dfe_name");
            IsoCode = entity.GetAttributeValue<string>("dfe_countrykey");
        }
    }
}

