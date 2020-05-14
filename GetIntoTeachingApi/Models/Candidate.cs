using System;
using System.Collections.Generic;
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
        public List<CandidateQualification> Qualifications { get; set; }
        public List<CandidatePastTeachingPosition> PastTeachingPositions { get; set; }
        [SwaggerSchema("Set to schedule a phone call.", WriteOnly = true)]
        public PhoneCall PhoneCall { get; set; }
        [SwaggerSchema("Set to update the accepted privacy policy.", WriteOnly = true)]
        public CandidatePrivacyPolicy PrivacyPolicy { get; set; }

        public Candidate()
        {
            Qualifications = new List<CandidateQualification>();
            PastTeachingPositions = new List<CandidatePastTeachingPosition>();
        }

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

        public Entity PopulateEntity(Entity entity)
        {
            if (PreferredTeachingSubjectId != null)
                entity["dfe_preferredteachingsubject01"] = new EntityReference("dfe_teachingsubjectlist", (Guid) PreferredTeachingSubjectId);

            if (PreferredEducationPhaseId != null)
                entity["dfe_preferrededucationphase01"] = new OptionSetValue((int) PreferredEducationPhaseId);

            if (LocationId != null)
                entity["dfe_isinuk"] = new OptionSetValue((int)LocationId);

            if (InitialTeacherTrainingYearId != null)
                entity["dfe_ittyear"] = new OptionSetValue((int) InitialTeacherTrainingYearId);

            entity["emailaddress1"] = Email;
            entity["firstname"] = FirstName;
            entity["lastname"] = LastName;
            entity["birthdate"] = DateOfBirth;
            entity["telephone1"] = Telephone;

            if (Address != null)
            {
                entity["address1_line1"] = Address.Line1;
                entity["address1_line2"] = Address.Line2;
                entity["address1_line3"] = Address.Line3;
                entity["address1_city"] = Address.City;
                entity["address1_stateorprovince"] = Address.State;
                entity["address1_postalcode"] = Address.Postcode;
            }

            return entity;
        }
    }
}
