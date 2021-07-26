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

            RuleFor(operation => operation.ReadableId)
                .Must((te, _) => BeUniqueReadableId(te))
                .WithMessage("Must be unique");
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
