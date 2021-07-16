using FluentValidation;
using GetIntoTeachingApi.Models.GetIntoTeaching;
using GetIntoTeachingApi.Models.Validators.Crm;
using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApi.Models.Validators.GetIntoTeaching
{
    public class MailingListAddMemberValidator : AbstractValidator<MailingListAddMember>
    {
        public MailingListAddMemberValidator(IStore store, IDateTimeProvider dateTime)
        {
            RuleFor(request => request.FirstName).NotEmpty();
            RuleFor(request => request.LastName).NotEmpty();
            RuleFor(request => request.Email).NotEmpty();
            RuleFor(request => request.AcceptedPolicyId).NotEmpty();
            RuleFor(request => request.ConsiderationJourneyStageId).NotNull();
            RuleFor(request => request.DegreeStatusId).NotNull();
            RuleFor(request => request.PreferredTeachingSubjectId).NotNull();

            RuleFor(request => request.Candidate).SetValidator(new CandidateValidator(store, dateTime));
        }
    }
}