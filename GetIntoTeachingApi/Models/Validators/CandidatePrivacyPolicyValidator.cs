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

            RuleFor(candidate => candidate.AcceptedPolicy).NotNull().SetValidator(new PrivacyPolicyValidator(crm));
        }
    }
}