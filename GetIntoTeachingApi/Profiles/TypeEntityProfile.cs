using AutoMapper;
using GetIntoTeachingApi.Models;
using Microsoft.Xrm.Sdk;
using static Microsoft.PowerPlatform.Cds.Client.CdsServiceClient;

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

            CreateMap<PickListItem, TypeEntity>()
                .ForMember(dest =>
                    dest.Id,
                    opt => opt.MapFrom(src => src.PickListItemId)
                )
                .ForMember(dest =>
                    dest.Value,
                    opt => opt.MapFrom(src => src.DisplayLabel)
                );
        }
    }
}
