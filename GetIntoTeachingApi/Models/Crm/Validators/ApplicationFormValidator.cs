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
                .NotNull()
                .SetValidator(new PickListItemIdValidator<ApplicationForm>("dfe_applyapplicationform", "dfe_applyphase", store));
            RuleFor(form => form.StatusId)
                .NotNull()
                .SetValidator(new PickListItemIdValidator<ApplicationForm>("dfe_applyapplicationform", "dfe_applystatus", store));
            RuleFor(form => form.RecruitmentCycleYearId)
                .NotNull()
                .SetValidator(new PickListItemIdValidator<ApplicationForm>("dfe_applyapplicationform", "dfe_recruitmentyear", store));
        }
    }
}
