using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApi.Models.Validators
{
    public class CandidatePrivacyPolicyValidator : AbstractValidator<CandidatePrivacyPolicy>
    {
        private readonly ICrmService _crm;

        public CandidatePrivacyPolicyValidator(ICrmService crm)
        {
            _crm = crm;

            RuleFor(privacyPolicy => privacyPolicy.AcceptedPolicyId)
                .Must(id => PrivacyPolicyIds().Contains(id))
                .WithMessage("Must be a valid privacy policy.");
        }

        private IEnumerable<Guid?> PrivacyPolicyIds()
        {
            return _crm.GetPrivacyPolicies().Select(policy => (Guid?)policy.Id);
        }
    }
}