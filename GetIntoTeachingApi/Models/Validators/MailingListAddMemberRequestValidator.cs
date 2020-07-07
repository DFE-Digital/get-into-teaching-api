using FluentValidation;
using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApi.Models.Validators
{
    public class MailingListAddMemberRequestValidator : AbstractValidator<MailingListAddMemberRequest>
    {
        public MailingListAddMemberRequestValidator(IStore store)
        {
            RuleFor(request => request.AddressPostcode).NotEmpty();

            RuleFor(request => request.Candidate).SetValidator(new CandidateValidator(store));
        }
    }
}