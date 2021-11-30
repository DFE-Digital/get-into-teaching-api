using System.Text.RegularExpressions;
using FluentValidation;
using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApi.Models.GetIntoTeaching.Validators
{
    public class TeachingEventUpsertOperationValidator : AbstractValidator<TeachingEventUpsertOperation>
    {
        private readonly ICrmService _crm;

        public TeachingEventUpsertOperationValidator(ICrmService crm)
        {
            _crm = crm;

            // TODO: the regex we use here has been relaxed be due to a number of
            // pre-existing events that would otherwise fail the validation.
            // We should look to revert it back to the stricter regex in the
            // future (when these events have since passed - 24/6/22).
            RuleFor(operation => operation.ReadableId)
                .Must((te, _) => BeUniqueReadableId(te))
                .WithMessage("Must be unique")
                .Matches(new Regex(@"\A[\w\-:\(\)&'–]+\Z"));
        }

        private bool BeUniqueReadableId(TeachingEventUpsertOperation operation)
        {
            var existingTeachingEvent = _crm.GetTeachingEvent(operation.ReadableId);

            if (existingTeachingEvent == null)
            {
                return true;
            }

            if (existingTeachingEvent.Id == operation.Id)
            {
                return true;
            }

            return false;
        }
    }
}
