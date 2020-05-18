using FluentValidation;
using System;

namespace GetIntoTeachingApi.Models.Validators
{
    public class PhoneCallValidator : AbstractValidator<PhoneCall>
    {
        public PhoneCallValidator()
        {
            RuleFor(phoneCall => phoneCall.ScheduledAt).GreaterThan(candidate => DateTime.Now);
            RuleFor(phoneCall => phoneCall.Telephone).NotEmpty().MaximumLength(50);
        }
    }
}