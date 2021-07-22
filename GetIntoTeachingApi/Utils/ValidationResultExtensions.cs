using System.Linq;
using FluentValidation;
using FluentValidation.Results;

namespace GetIntoTeachingApi.Utils
{
    public static class ValidationResultExtensions
    {
        public static ValidationResult SurfaceErrorsOnMatchingProperties(
            this ValidationResult result, IValidationContext validationContext)
        {
            var model = validationContext.InstanceToValidate;
            result.Errors.ForEach(error => RemapError(error, model));
            return result;
        }

        private static void RemapError(ValidationFailure error, object model)
        {
            var errorPropertyName = error.PropertyName.Split('.').Last();
            var modelHasMatchingProperty = model
                .GetType()
                .GetProperties()
                .Any(property => property.Name == errorPropertyName);

            if (modelHasMatchingProperty)
            {
                error.PropertyName = errorPropertyName;
            }
        }
    }
}
