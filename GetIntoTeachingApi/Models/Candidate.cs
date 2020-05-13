using System;
using GetIntoTeachingApi.Profiles;
using Microsoft.Xrm.Sdk;
using Swashbuckle.AspNetCore.Annotations;

namespace GetIntoTeachingApi.Models
{
    public class Candidate
    {
        public Guid? Id { get; set; }
        public Guid? PreferredTeachingSubjectId { get; set; }
        public int? PreferredEducationPhaseId { get; set; }
        public int? LocationId { get; set; }
        public int? InitialTeacherTrainingYearId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Telephone { get; set; }
        public Address Address { get; set; }
        public CandidateQualification[] Qualifications { get; set; }
        public CandidatePastTeachingPosition[] PastTeachingPositions { get; set; }
        public DateTime? PhoneCallScheduledStartAt { get; set; }
        public Guid? AcceptedPrivacyPolicyId { get; set; }

        public Candidate() { }

        public Candidate(Entity entity)
        {
            Id = entity.Id;
            PreferredTeachingSubjectId = entity.GetAttributeValue<EntityReference>("dfe_preferredteachingsubject01")?.Id;
            PreferredEducationPhaseId = entity.GetAttributeValue<OptionSetValue>("dfe_preferrededucationphase01")?.Value;
            LocationId = entity.GetAttributeValue<OptionSetValue>("dfe_isinuk")?.Value;
            InitialTeacherTrainingYearId = entity.GetAttributeValue<OptionSetValue>("dfe_ittyear")?.Value;
            Email = entity.GetAttributeValue<string>("emailaddress1");
            FirstName = entity.GetAttributeValue<string>("firstname");
            LastName = entity.GetAttributeValue<string>("lastname");
            DateOfBirth = entity.GetAttributeValue<DateTime>("birthdate");
            Telephone = entity.GetAttributeValue<string>("telephone1");
            Address = new Address()
            {
                Line1 = entity.GetAttributeValue<string>("address1_line1"),
                Line2 = entity.GetAttributeValue<string>("address1_line2"),
                Line3 = entity.GetAttributeValue<string>("address1_line3"),
                City = entity.GetAttributeValue<string>("address1_city"),
                State = entity.GetAttributeValue<string>("address1_stateorprovince"),
                Postcode = entity.GetAttributeValue<string>("address1_postalcode"),
            };
        }

        public Entity ToEntity()
        {
            var entity = new Entity("contact");

            if (Id != null) entity.Id = (Guid)Id;
            if (PreferredTeachingSubjectId != null)
                entity.Attributes.Add("dfe_preferredteachingsubject01",
                    new EntityReference("dfe_teachingsubjectlist", (Guid)PreferredTeachingSubjectId));
            if (PreferredEducationPhaseId != null) 
                entity.Attributes.Add("dfe_preferrededucationphase01", new OptionSetValue((int)PreferredEducationPhaseId));
            if (LocationId != null) 
                entity.Attributes.Add("dfe_isinuk", new OptionSetValue((int)LocationId));
            if (InitialTeacherTrainingYearId != null) 
                entity.Attributes.Add("dfe_ittyear", new OptionSetValue((int)InitialTeacherTrainingYearId));

            entity.Attributes.Add("emailaddress1", Email);
            entity.Attributes.Add("firstname", FirstName);
            entity.Attributes.Add("lastname", LastName);
            entity.Attributes.Add("birthdate", DateOfBirth);
            entity.Attributes.Add("telephone1", Telephone);
            entity.Attributes.Add("address1_line1", Address.Line1);
            entity.Attributes.Add("address1_line2", Address.Line2);
            entity.Attributes.Add("address1_line3", Address.Line3);
            entity.Attributes.Add("address1_city", Address.City);
            entity.Attributes.Add("address1_stateorprovince", Address.State);
            entity.Attributes.Add("address1_postalcode", Address.Postcode);

            return entity;
        }
    }
}
