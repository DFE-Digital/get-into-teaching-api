using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApi.Models.Validators
{
    public class PrivacyPolicyValidator : AbstractValidator<PrivacyPolicy>
    {
        private readonly ICrmService _crm;

        public PrivacyPolicyValidator(ICrmService crm)
        {
            _crm = crm;

            RuleFor(privacyPolicy => privacyPolicy.Id)
                .Must(id => PrivacyPolicyIds().Contains(id))
                .WithMessage("Must be a valid privacy policy.");
        }

        private IEnumerable<Guid?> PrivacyPolicyIds()
        {
            return _crm.GetPrivacyPolicies().Select(policy => (Guid?)policy.Id);
        }
    }
}