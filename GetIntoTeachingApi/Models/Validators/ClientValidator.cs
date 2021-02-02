using FluentValidation;

namespace GetIntoTeachingApi.Models.Validators
{
    public class ClientValidator : AbstractValidator<Client>
    {
        private const string ApiKeyPrefixRegex = "\\A[A-Z][A-Z_]+[A-Z]\\z";
        private const string RoleRegex = "\\A([A-Z][a-z0-9]+)+\\z";

        public ClientValidator()
        {
            RuleFor(client => client.Name).NotEmpty();
            RuleFor(client => client.Description).NotEmpty();
            RuleFor(client => client.ApiKeyPrefix).NotEmpty().Matches(ApiKeyPrefixRegex);
            RuleFor(client => client.Role).NotEmpty().Matches(RoleRegex);
        }
    }
}