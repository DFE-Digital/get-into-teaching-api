using System;
using FluentValidation;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Validators;

namespace GetIntoTeachingApi.Models.Crm.Validators
{
    public class ApplicationChoiceValidator : AbstractValidator<ApplicationChoice>
    {
        public ApplicationChoiceValidator(IStore store)
        {
            RuleFor(choice => choice.FindApplyId).NotEmpty();
            RuleFor(choice => choice.CourseName).NotEmpty();
            RuleFor(choice => choice.CourseId).NotEmpty().Must(courseId => Guid.TryParse(courseId, out _));
            RuleFor(choice => choice.Provider).NotEmpty();
            RuleFor(choice => choice.StatusId)
                .NotNull()
                .SetValidator(new PickListItemIdValidator<ApplicationChoice>("dfe_applyapplicationchoice", "dfe_applicationchoicestatus", store));
        }
    }
}
