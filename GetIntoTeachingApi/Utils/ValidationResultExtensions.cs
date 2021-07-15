using System.Linq;
using FluentValidation;
using FluentValidation.Results;

namespace GetIntoTeachingApi.Utils
{
    public static class ValidationResultExtensions
    {
        public static void MapRelatedModelErrorsToRequestModel(
            this ValidationResult result, IValidationContext validationContext, string relatedModelName)
        {
            var requestModel = validationContext.InstanceToValidate;
            foreach (var error in result.Errors)
            {
                string[] splitPropertyName = error.PropertyName.Split('.');
                string propertyName = splitPropertyName.Last();

                bool relatedModelProperty = splitPropertyName.First() == relatedModelName;
                bool requestModelHasProperty = requestModel
                    .GetType()
                    .GetProperties()
                    .Any(property => property.Name == propertyName);

                if (relatedModelProperty && requestModelHasProperty)
                {
                    error.PropertyName = propertyName;
                }
            }
        }
    }
}
