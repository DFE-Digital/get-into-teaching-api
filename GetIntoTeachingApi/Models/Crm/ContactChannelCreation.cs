using System;
using FluentValidation;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Services;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Models.Crm
{
    [SwaggerIgnore]
    [Entity("dfe_contactchannelcreation")]
    public class ContactChannelCreation : BaseModel
    {
        public enum CreationChannelSource
        {
            Apply = 222750000,
            CheckinApp = 222750001,
            ContactCentre = 222750002,
            GITWebsite = 222750003,
            Highfliers = 222750004,
            HPITT = 222750005,
            Internships = 222750006,
            Legacy = 222750007,
            OnCampus = 222750008, 
            PaidAdvertising = 222750009,
            PaidSearch = 222750010,
            PaidSocial = 222750011,
            Pipeline = 222750012,
            SchoolExperience = 222750013, 
            Scholarships = 222750014
        }

        public enum CreationChannelService
        {
            CreatedOnApply = 222750000,
            CreatedOnSchoolExperience = 222750001,
            CreatedOnScholarships = 222750002,
            CreatedOnInternships = 222750003,
            CreatedOnHPITT = 222750004,
            CreatedOnHighfliers = 222750011,
            ExploreTeachingAdviserService = 222750005,
            Events = 222750006,
            MailingList = 222750007,
            PaidSearch = 222750008,
            ReturnToTeachingAdviserService = 222750009,
            TeacherTrainingAdviserService = 222750010
        }
        
        public enum CreationChannelActivity
        {
            BrandAmbassadorActivity = 222750000,
            BritishCouncil = 222750001,
            BRFS = 222750002,
            BCS = 222750003,
            CareersEvent = 222750004,
            CTP = 222750005,
            DebateMate = 222750006,
            EngineersTeachPhysics = 222750007,
            FreshersFairs = 222750008,
            F2F = 222750009,
            GradFairs = 222750010,
            InstituteOfPhysics = 222750011,
            IMECHE = 222750012,
            IMA = 222750013,
            NTP = 222750014,
            OnsiteActivationDays = 222750015,
            Over18CareersEvent = 222750016,
            QuickfireSignUpOnApply = 222750017,
            RefreshersFairs = 222750018,
            RussellGroup6 = 222750019,
            RCS = 222750020,
            StudentUnionMedia = 222750021,
            StudentRooms = 222750022,
            ServiceLeaver = 222750023,
            Webinar = 222750024
        }
        
        
        [EntityField("createdby", typeof(EntityReference), "systemuser")]
        public Guid? CreatedBy { get; set; }
        
        [EntityField("dfe_contactid", typeof(EntityReference), "contact")]
        public Guid ContactId { get; set; }

        [EntityField("dfe_creationchannel")] 
        public bool? CreationChannel { get; set; } = false;
        
        [EntityField("dfe_creationchannelsource", typeof(OptionSetValue))]
        public int? CreationChannelSourceId { get; set; }
        
        [EntityField("dfe_creationchannelservice", typeof(OptionSetValue))]
        public int? CreationChannelServiceId { get; set; }
        
        [EntityField("dfe_creationchannelactivities", typeof(OptionSetValue))]
        public int? CreationChannelActivityId { get; set; }
        
        public ContactChannelCreation() : base()
        {
        }
        
        public ContactChannelCreation(Entity entity, ICrmService crm, IServiceProvider serviceProvider)
            : base(entity, crm, serviceProvider)
        {
        }
    }
}