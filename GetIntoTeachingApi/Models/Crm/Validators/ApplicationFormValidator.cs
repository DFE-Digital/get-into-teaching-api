using FluentValidation;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Validators;

namespace GetIntoTeachingApi.Models.Crm.Validators
{
    public class ApplicationFormValidator : AbstractValidator<ApplicationForm>
    {
        public ApplicationFormValidator(IStore store)
        {
            RuleFor(form => form.FindApplyId).NotEmpty();
            RuleFor(form => form.PhaseId)
                .SetValidator(new PickListItemIdValidator<ApplicationForm>("dfe_applyapplicationform", "dfe_candidateapplyphase", store))
                .Unless(form => form.PhaseId == null);
        }
    }
}