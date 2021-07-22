using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApi.Models.Crm.Validators
{
    public class CandidatePrivacyPolicyValidator : AbstractValidator<CandidatePrivacyPolicy>
    {
        private readonly IStore _store;

        public CandidatePrivacyPolicyValidator(IStore store)
        {
            _store = store;

            RuleFor(privacyPolicy => privacyPolicy.AcceptedPolicyId)
                .Must(id => PrivacyPolicyIds().Contains(id))
                .WithMessage("Must be a valid privacy policy.");
        }

        private IEnumerable<Guid?> PrivacyPolicyIds()
        {
            return _store.GetPrivacyPolicies().Select(policy => policy.Id);
        }
    }
}