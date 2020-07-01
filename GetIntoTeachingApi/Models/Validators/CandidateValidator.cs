using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Validators;
using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApi.Models.Validators
{
    public class CandidateValidator : AbstractValidator<Candidate>
    {
        private readonly IStore _store;

        public CandidateValidator(IStore store)
        {
            _store = store;

            RuleFor(candidate => candidate.FirstName).NotEmpty().MaximumLength(256);
            RuleFor(candidate => candidate.LastName).NotEmpty().MaximumLength(256);
            RuleFor(candidate => candidate.Email).NotEmpty().EmailAddress(EmailValidationMode.AspNetCoreCompatible).MaximumLength(100);
            RuleFor(candidate => candidate.DateOfBirth).NotNull().LessThan(candidate => DateTime.Now);
            RuleFor(candidate => candidate.Telephone).NotEmpty().MaximumLength(50);
            RuleFor(candidate => candidate.AddressLine1).NotEmpty().MaximumLength(1024);
            RuleFor(candidate => candidate.AddressLine2).MaximumLength(1024);
            RuleFor(candidate => candidate.AddressLine3).MaximumLength(1024);
            RuleFor(candidate => candidate.AddressCity).NotEmpty().MaximumLength(128);
            RuleFor(candidate => candidate.AddressState).NotEmpty().MaximumLength(128);
            RuleFor(candidate => candidate.AddressPostcode).NotEmpty().MaximumLength(40);

            RuleFor(candidate => candidate.PhoneCall).SetValidator(new PhoneCallValidator(store)).Unless(candidate => candidate.PhoneCall == null);
            RuleFor(candidate => candidate.PrivacyPolicy).NotNull().SetValidator(new CandidatePrivacyPolicyValidator(store));
            RuleForEach(candidate => candidate.Qualifications).SetValidator(new CandidateQualificationValidator(store));
            RuleForEach(candidate => candidate.PastTeachingPositions).SetValidator(new CandidatePastTeachingPositionValidator(store));

            RuleFor(candidate => candidate.PreferredTeachingSubjectId)
                .Must(id => PreferredTeachingSubjectIds().Contains(id.ToString()))
                .Unless(candidate => candidate.PreferredTeachingSubjectId == null)
                .WithMessage("Must be a valid teaching subject.");
            RuleFor(candidate => candidate.CountryId)
                .Must(id => CountryIds().Contains(id.ToString()))
                .WithMessage("Must be a valid country.");
            RuleFor(candidate => candidate.PreferredEducationPhaseId)
                .Must(id => PreferredEducationPhaseIds().Contains(id.ToString()))
                .Unless(candidate => candidate.PreferredEducationPhaseId == null)
                .WithMessage("Must be a valid candidate education phase.");
            RuleFor(candidate => candidate.InitialTeacherTrainingYearId)
                .Must(id => InitialTeacherTrainingYearIds().Contains(id.ToString()))
                .Unless(candidate => candidate.InitialTeacherTrainingYearId == null)
                .WithMessage("Must be a valid candidate initial teacher training year.");
            RuleFor(candidate => candidate.ChannelId)
                .Must(id => ChannelIds().Contains(id.ToString()))
                .WithMessage("Must be a valid candidate channel.");
            RuleFor(candidate => candidate.HasGcseEnglishId)
                .Must(id => GcseStatusIds().Contains(id.ToString()))
                .Unless(candidate => candidate.HasGcseEnglishId == null)
                .WithMessage("Must be a valid candidate GCSE status.");
            RuleFor(candidate => candidate.HasGcseMathsId)
                .Must(id => GcseStatusIds().Contains(id.ToString()))
                .Unless(candidate => candidate.HasGcseMathsId == null)
                .WithMessage("Must be a valid candidate GCSE status.");
            RuleFor(candidate => candidate.HasGcseScienceId)
                .Must(id => GcseStatusIds().Contains(id.ToString()))
                .Unless(candidate => candidate.HasGcseScienceId == null)
                .WithMessage("Must be a valid candidate GCSE status.");
        }

        private IEnumerable<string> PreferredTeachingSubjectIds()
        {
            return _store.GetLookupItems("dfe_teachingsubjectlist").Select(subject => subject.Id);
        }

        private IEnumerable<string> CountryIds()
        {
            return _store.GetLookupItems("dfe_country").Select(country => country.Id);
        }

        private IEnumerable<string> PreferredEducationPhaseIds()
        {
            return _store.GetPickListItems("contact", "dfe_preferrededucationphase01").Select(phase => phase.Id);
        }

        private IEnumerable<string> InitialTeacherTrainingYearIds()
        {
            return _store.GetPickListItems("contact", "dfe_ittyear").Select(year => year.Id);
        }

        private IEnumerable<string> ChannelIds()
        {
            return _store.GetPickListItems("contact", "dfe_channelcreation").Select(channel => channel.Id);
        }

        private IEnumerable<string> GcseStatusIds()
        {
            return _store.GetPickListItems("contact", "dfe_hasgcseenglish").Select(status => status.Id);
        }
    }
}