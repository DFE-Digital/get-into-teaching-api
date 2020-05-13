using AutoMapper;
using GetIntoTeachingApi.Models;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Profiles
{
    public class CandidateQualificationProfile : Profile
    {
        public CandidateQualificationProfile()
        {
            CreateMap<Entity, CandidateQualification>()
                .ForMember(dest => dest.CategoryId,
                    opt => opt.MapFrom(src =>
                        src.GetAttributeValue<OptionSetValue>("dfe_category").Value))
                .ForMember(dest => dest.TypeId,
                    opt => opt.MapFrom(src => src.GetAttributeValue<OptionSetValue>("dfe_type").Value))
                .ForMember(dest => dest.DegreeStatusId,
                    opt => opt.MapFrom(src => src.GetAttributeValue<OptionSetValue>("dfe_degreestatus").Value));
        }
    }
}
