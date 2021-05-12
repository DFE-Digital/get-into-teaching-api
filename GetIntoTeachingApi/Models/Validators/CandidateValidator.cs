using System;
using System.Linq;
using FluentValidation;
using FluentValidation.Validators;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Validators;

namespace GetIntoTeachingApi.Models.Validators
{
    public class CandidateValidator : AbstractValidator<Candidate>
    {
        private readonly string[] _validEligibilityRulesPassedValues = new[] { "true", "false" };

        public CandidateValidator(IStore store, IDateTimeProvider dateTime)
        {
            RuleFor(candidate => candidate.FirstName).NotEmpty().MaximumLength(256);
            RuleFor(candidate => candidate.LastName).NotEmpty().MaximumLength(256);
            RuleFor(candidate => candidate.Email).NotEmpty().EmailAddress(EmailValidationMode.AspNetCoreCompatible).MaximumLength(100);
            RuleFor(candidate => candidate.DateOfBirth).LessThan(candidate => dateTime.UtcNow);
            RuleFor(candidate => candidate.AddressTelephone).MinimumLength(5).MaximumLength(20).Matches(@"^[^a-zA-Z]+$");
            RuleFor(candidate => candidate.AddressLine1).MaximumLength(1024);
            RuleFor(candidate => candidate.AddressLine2).MaximumLength(1024);
            RuleFor(candidate => candidate.AddressCity).MaximumLength(128);
            RuleFor(candidate => candidate.AddressPostcode)
                .SetValidator(new PostcodeValidator())
                .Unless(candidate => candidate.AddressPostcode == null);
            RuleFor(candidate => candidate.EligibilityRulesPassed)
                .Must(value => _validEligibilityRulesPassedValues.Contains(value))
                .Unless(candidate => candidate.EligibilityRulesPassed == null)
                .WithMessage("Must be true or false (as string values).");

            RuleFor(candidate => candidate.PhoneCall).SetValidator(new PhoneCallValidator(store)).Unless(candidate => candidate.PhoneCall == null);
            RuleFor(candidate => candidate.PrivacyPolicy).SetValidator(new CandidatePrivacyPolicyValidator(store));
            RuleForEach(candidate => candidate.Qualifications).SetValidator(new CandidateQualificationValidator(store));
            RuleForEach(candidate => candidate.PastTeachingPositions).SetValidator(new CandidatePastTeachingPositionValidator(store));
            RuleForEach(candidate => candidate.TeachingEventRegistrations).SetValidator(new TeachingEventRegistrationValidator(store));

            RuleFor(candidate => candidate.PreferredTeachingSubjectId)
                .SetValidator(new LookupItemIdValidator("dfe_teachingsubjectlist", store))
                .Unless(candidate => candidate.PreferredTeachingSubjectId == null);
            RuleFor(candidate => candidate.CountryId)
                .SetValidator(new LookupItemIdValidator("dfe_country", store))
                .Unless(candidate => candidate.CountryId == null);
            RuleFor(candidate => candidate.PreferredEducationPhaseId)
                .SetValidator(new PickListItemIdValidator("contact", "dfe_preferrededucationphase01", store))
                .Unless(candidate => candidate.PreferredEducationPhaseId == null);
            RuleFor(candidate => candidate.InitialTeacherTrainingYearId)
                .SetValidator(new PickListItemIdValidator("contact", "dfe_ittyear", store))
                .Unless(candidate => candidate.InitialTeacherTrainingYearId == null);
            RuleFor(candidate => candidate.ChannelId)
                .SetValidator(new PickListItemIdValidator("contact", "dfe_channelcreation", store))
                .Unless(candidate => candidate.Id != null);
            RuleFor(candidate => candidate.HasGcseEnglishId)
                .SetValidator(new PickListItemIdValidator("contact", "dfe_websitehasgcseenglish", store))
                .Unless(candidate => candidate.HasGcseEnglishId == null);
            RuleFor(candidate => candidate.HasGcseMathsId)
                .SetValidator(new PickListItemIdValidator("contact", "dfe_websitehasgcseenglish", store))
                .Unless(candidate => candidate.HasGcseMathsId == null);
            RuleFor(candidate => candidate.HasGcseScienceId)
                .SetValidator(new PickListItemIdValidator("contact", "dfe_websitehasgcseenglish", store))
                .Unless(candidate => candidate.HasGcseScienceId == null);
            RuleFor(candidate => candidate.PlanningToRetakeGcseScienceId)
                .SetValidator(new PickListItemIdValidator("contact", "dfe_websiteplanningretakeenglishgcse", store))
                .Unless(candidate => candidate.PlanningToRetakeGcseScienceId == null);
            RuleFor(candidate => candidate.PlanningToRetakeGcseEnglishId)
                .SetValidator(new PickListItemIdValidator("contact", "dfe_websiteplanningretakeenglishgcse", store))
                .Unless(candidate => candidate.PlanningToRetakeGcseEnglishId == null);
            RuleFor(candidate => candidate.PlanningToRetakeGcseMathsId)
                .SetValidator(new PickListItemIdValidator("contact", "dfe_websiteplanningretakeenglishgcse", store))
                .Unless(candidate => candidate.PlanningToRetakeGcseMathsId == null);
            RuleFor(candidate => candidate.ConsiderationJourneyStageId)
                .SetValidator(new PickListItemIdValidator("contact", "dfe_websitewhereinconsiderationjourney", store))
                .Unless(candidate => candidate.ConsiderationJourneyStageId == null);
            RuleFor(candidate => candidate.TypeId)
                .SetValidator(new PickListItemIdValidator("contact", "dfe_typeofcandidate", store))
                .Unless(candidate => candidate.TypeId == null);
            RuleFor(candidate => candidate.AssignmentStatusId)
                .SetValidator(new PickListItemIdValidator("contact", "dfe_candidatestatus", store))
                .Unless(candidate => candidate.AssignmentStatusId == null);
            RuleFor(candidate => candidate.AdviserEligibilityId)
                .SetValidator(new PickListItemIdValidator("contact", "dfe_iscandidateeligibleforadviser", store))
                .Unless(candidate => candidate.AdviserEligibilityId == null);
            RuleFor(candidate => candidate.AdviserRequirementId)
                .SetValidator(new PickListItemIdValidator("contact", "dfe_websitewhereinconsiderationjourney", store))
                .Unless(candidate => candidate.AdviserRequirementId == null);
            RuleFor(candidate => candidate.EventsSubscriptionChannelId)
                .SetValidator(new PickListItemIdValidator("contact", "dfe_gitiseventsservicesubscriptionchannel", store))
                .Unless(candidate => candidate.EventsSubscriptionChannelId == null);
            RuleFor(candidate => candidate.MailingListSubscriptionChannelId)
                .SetValidator(new PickListItemIdValidator("contact", "dfe_gitismlservicesubscriptionchannel", store))
                .Unless(candidate => candidate.MailingListSubscriptionChannelId == null);
        }
    }
}