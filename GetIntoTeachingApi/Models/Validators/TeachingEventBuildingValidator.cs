using FluentValidation;
using GetIntoTeachingApi.Validators;

namespace GetIntoTeachingApi.Models.Validators
{
    public class TeachingEventBuildingValidator : AbstractValidator<TeachingEventBuilding>
    {
        public TeachingEventBuildingValidator()
        {
            RuleFor(building => building.Venue).NotEmpty();
            RuleFor(building => building.AddressPostcode).SetValidator(new PostcodeValidator());
        }
    }
}
