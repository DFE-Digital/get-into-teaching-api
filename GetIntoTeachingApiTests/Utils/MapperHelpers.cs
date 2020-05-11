using AutoMapper;
using GetIntoTeachingApi.Profiles;

namespace GetIntoTeachingApiTests.Utils
{
    public static class MapperHelpers
    {
        public static Mapper CreateMapper()
        {
            var config = new MapperConfiguration(config => {
                config.AddProfile<TypeEntityProfile>();
                config.AddProfile<PrivacyPolicyProfile>();
                config.AddProfile<CandidateProfile>();
            });

            return new Mapper(config);
        }
    }
}
