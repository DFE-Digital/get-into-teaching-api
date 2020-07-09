using FluentValidation;
using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApi.Models.Validators
{
    public class MailingListAddMemberValidator : AbstractValidator<MailingListAddMember>
    {
        public MailingListAddMemberValidator(IStore store)
        {
            RuleFor(request => request.AddressPostcode).NotEmpty();

            RuleFor(request => request.Candidate).SetValidator(new CandidateValidator(store));
        }
    }
}