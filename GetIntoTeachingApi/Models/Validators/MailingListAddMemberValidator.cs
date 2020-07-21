using FluentValidation;
using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApi.Models.Validators
{
    public class MailingListAddMemberValidator : AbstractValidator<MailingListAddMember>
    {
        public MailingListAddMemberValidator(IStore store)
        {
            RuleFor(request => request.FirstName).NotEmpty();
            RuleFor(request => request.LastName).NotEmpty();
            RuleFor(request => request.Email).NotEmpty();
            RuleFor(request => request.AddressPostcode).NotEmpty();
            RuleFor(request => request.AcceptedPolicyId).NotEmpty();
            RuleFor(request => request.ConsiderationJourneyStageId).NotNull();
            RuleFor(request => request.CallbackInformation).NotEmpty()
                .When(request => request.Telephone != null);

            RuleFor(request => request.Candidate).SetValidator(new CandidateValidator(store));
        }
    }
}