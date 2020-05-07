using AutoMapper;
using GetIntoTeachingApi.Profiles;
using Xunit;

namespace GetIntoTeachingApiTests.Profiles
{
    public class ProfileTests
    {
        [Fact]
        public void TypeEntityProfile_HasValidConfiguration()
        {
            var config = new MapperConfiguration(config => {
                config.AddProfile<TypeEntityProfile>();
            });
            config.AssertConfigurationIsValid();
        }
    }
}
