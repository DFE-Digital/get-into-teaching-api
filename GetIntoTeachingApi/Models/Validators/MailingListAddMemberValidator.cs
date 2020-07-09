using FluentValidation;
using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApi.Models.Validators
{
    public class MailingListAddMemberValidator : AbstractValidator<MailingListAddMember>
    {
        public MailingListAddMemberValidator(IStore store)
        {
            RuleFor(request => request.AddressPostcode).NotEmpty();
            RuleFor(request => request.PreferredTeachingSubjectId).NotEmpty();
            RuleFor(request => request.AcceptedPolicyId).NotEmpty();
            RuleFor(request => request.DescribeYourselfOptionId).NotEmpty();
            RuleFor(request => request.ConsiderationJourneyStageId).NotEmpty();
            RuleFor(request => request.UkDegreeGradeId).NotEmpty();

            RuleFor(request => request.Candidate).SetValidator(new CandidateValidator(store));
        }
    }
}