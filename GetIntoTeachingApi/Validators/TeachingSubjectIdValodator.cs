using System;
using System.Linq;
using FluentValidation;
using FluentValidation.Validators;
using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApi.Validators
{
    public class TeachingSubjectIdValidator<T> : PropertyValidator<T, Guid?>
    {
        private readonly IStore _store;

        public TeachingSubjectIdValidator(IStore store)
            : base()
        {
            _store = store;
        }

        public override bool IsValid(ValidationContext<T> context, Guid? value)
        {
            var exists = _store.GetTeachingSubjects().Any(i => i.Id == value);

            if (exists)
            {
                return true;
            }

            context.MessageFormatter.AppendArgument("PropertyName", context.PropertyName);

            return false;
        }

        public override string Name => "TeachingSubjectIdValidator";

        protected override string GetDefaultMessageTemplate(string errorCode) => "{PropertyName} must be a valid teaching subject id.";
    }
}
