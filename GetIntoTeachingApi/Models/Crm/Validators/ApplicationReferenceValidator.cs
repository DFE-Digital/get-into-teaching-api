using FluentValidation;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Validators;

namespace GetIntoTeachingApi.Models.Crm.Validators
{
    public class ApplicationReferenceValidator : AbstractValidator<ApplicationReference>
    {
        public ApplicationReferenceValidator(IStore store)
        {
            RuleFor(choice => choice.FindApplyId).NotEmpty();
            RuleFor(choice => choice.Type).NotEmpty();
            RuleFor(choice => choice.FeedbackStatusId)
                .NotNull()
                .SetValidator(new PickListItemIdValidator<ApplicationReference>("dfe_applyreference", "dfe_referencefeedbackstatus", store));
        }
    }
}
