using Bogus;
using GetIntoTeachingApi.Models;

namespace GetIntoTeachingApiTests.Models.GetIntoTeaching.Validators;

public class FakePickListItem: Faker<PickListItem>
{
    string[] entityNames = new []{ "contact", "dfe_contactchannelcreation" };
    public FakePickListItem()
    {
        RuleFor(p => p.Id, faker => faker.UniqueIndex + 1);
        RuleFor(p => p.EntityName, faker => faker.PickRandom(entityNames));
        RuleFor(p => p.AttributeName, faker => faker.Commerce.ProductAdjective());
    }

    public static Faker<PickListItem> Default = new Faker<FakePickListItem>();
}