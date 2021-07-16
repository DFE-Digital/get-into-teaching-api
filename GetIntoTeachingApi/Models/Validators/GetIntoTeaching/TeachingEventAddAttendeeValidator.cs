using FluentValidation;
using GetIntoTeachingApi.Models.GetIntoTeaching;
using GetIntoTeachingApi.Models.Validators.Crm;
using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApi.Models.Validators.GetIntoTeaching
{
    public class TeachingEventAddAttendeeValidator : AbstractValidator<TeachingEventAddAttendee>
    {
        public TeachingEventAddAttendeeValidator(IStore store, IDateTimeProvider dateTime)
        {
            RuleFor(request => request.FirstName).NotEmpty();
            RuleFor(request => request.LastName).NotEmpty();
            RuleFor(request => request.Email).NotEmpty();
            RuleFor(request => request.EventId).NotEmpty();
            RuleFor(request => request.AcceptedPolicyId).NotEmpty();
            RuleFor(request => request.ConsiderationJourneyStageId).NotNull().When(request => request.SubscribeToMailingList);
            RuleFor(request => request.DegreeStatusId).NotNull().When(request => request.SubscribeToMailingList);
            RuleFor(request => request.PreferredTeachingSubjectId).NotNull().When(request => request.SubscribeToMailingList);

            RuleFor(request => request.Candidate).SetValidator(new CandidateValidator(store, dateTime));
        }
    }
}