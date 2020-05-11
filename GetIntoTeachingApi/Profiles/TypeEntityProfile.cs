using AutoMapper;
using GetIntoTeachingApi.Models;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Profiles
{
    public class TypeEntityProfile : Profile
    {
        public TypeEntityProfile()
        {
            CreateMap<Entity, TypeEntity>().ForMember(dest => 
                dest.Value,
                opt => opt.MapFrom(src => src.GetAttributeValue<string>("dfe_name"))
            );
        }
    }
}
