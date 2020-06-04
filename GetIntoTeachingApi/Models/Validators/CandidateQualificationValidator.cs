using System.Collections.Generic;
using FluentValidation;
using GetIntoTeachingApi.Services;
using System.Linq;

namespace GetIntoTeachingApi.Models.Validators
{
    public class CandidateQualificationValidator : AbstractValidator<CandidateQualification>
    {
        private readonly ICrmService _crm;

        public CandidateQualificationValidator(ICrmService crm)
        {
            _crm = crm;

            RuleFor(qualification => qualification.TypeId)
                .Must(id => TypeIds().Contains(id.ToString()))
                .WithMessage("Must be a valid qualification type.");
            RuleFor(qualification => qualification.CategoryId)
                .Must(id => CategoryIds().Contains(id.ToString()))
                .Unless(qualification => qualification.CategoryId == null)
                .WithMessage("Must be a valid qualification category.");
            RuleFor(qualification => qualification.DegreeStatusId)
                .Must(id => StatusIds().Contains(id.ToString()))
                .Unless(qualification => qualification.DegreeStatusId == null)
                .WithMessage("Must be a valid qualification degree status.");
        }

        private IEnumerable<string> CategoryIds()
        {
            return _crm.GetPickListItems("dfe_qualification", "dfe_category").Select(category => category.Id);
        }

        private IEnumerable<string> TypeIds()
        {
            return _crm.GetPickListItems("dfe_qualification", "dfe_type").Select(type => type.Id);
        }

        private IEnumerable<string> StatusIds()
        {
            return _crm.GetPickListItems("dfe_qualification", "dfe_degreestatus").Select(status => status.Id);
        }
    }
}