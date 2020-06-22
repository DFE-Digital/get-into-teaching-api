using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Validators;
using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApi.Models.Validators
{
    public class MailingListAddMemberRequestValidator : AbstractValidator<MailingListAddMemberRequest>
    {
        private readonly IStore _store;

        public MailingListAddMemberRequestValidator(IStore store)
        {
            _store = store;

            RuleFor(request => request.FirstName).NotEmpty().MaximumLength(256);
            RuleFor(request => request.LastName).NotEmpty().MaximumLength(256);
            RuleFor(request => request.Email).NotEmpty().EmailAddress(EmailValidationMode.AspNetCoreCompatible).MaximumLength(100);
            RuleFor(request => request.Telephone).MaximumLength(50);
            RuleFor(request => request.AddressPostcode).NotEmpty().MaximumLength(40);

            RuleFor(request => request.PrivacyPolicy).NotEmpty().SetValidator(new CandidatePrivacyPolicyValidator(store));

            RuleFor(candidate => candidate.PreferredTeachingSubjectId)
                .Must(id => PreferredTeachingSubjectIds().Contains(id.ToString()))
                .WithMessage("Must be a valid teaching subject.");
        }

        private IEnumerable<string> PreferredTeachingSubjectIds()
        {
            return _store.GetLookupItems("dfe_teachingsubjectlist").Select(subject => subject.Id);
        }
    }
}
