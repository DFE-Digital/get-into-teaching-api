using FluentValidation;

namespace GetIntoTeachingApi.Models.Crm.Validators
{
    public class ApplicationInterviewValidator : AbstractValidator<ApplicationInterview>
    {
        public ApplicationInterviewValidator()
        {
            RuleFor(choice => choice.FindApplyId).NotEmpty();
        }
    }
}
