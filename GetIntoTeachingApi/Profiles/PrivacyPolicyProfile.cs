using AutoMapper;
using GetIntoTeachingApi.Models;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Profiles
{
    public class PrivacyPolicyProfile : Profile
    {
        public PrivacyPolicyProfile()
        {
            CreateMap<Entity, PrivacyPolicy>().ForMember(dest =>
                dest.Text,
                opt => opt.MapFrom(src => src.Attributes["dfe_details"])
            );
        }
    }
}
