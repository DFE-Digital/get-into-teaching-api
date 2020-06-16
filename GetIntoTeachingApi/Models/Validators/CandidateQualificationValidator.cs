using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using GetIntoTeachingApi.Services;

namespace GetIntoTeachingApi.Models.Validators
{
    public class CandidateQualificationValidator : AbstractValidator<CandidateQualification>
    {
        private readonly IStore _store;

        public CandidateQualificationValidator(IStore store)
        {
            _store = store;

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
            return _store.GetPickListItems("dfe_qualification", "dfe_category").Select(category => category.Id);
        }

        private IEnumerable<string> TypeIds()
        {
            return _store.GetPickListItems("dfe_qualification", "dfe_type").Select(type => type.Id);
        }

        private IEnumerable<string> StatusIds()
        {
            return _store.GetPickListItems("dfe_qualification", "dfe_degreestatus").Select(status => status.Id);
        }
    }
}