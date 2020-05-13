using AutoMapper;
using GetIntoTeachingApi.Models;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Profiles
{
    public class CandidatePastTeachingPositionProfile : Profile
    {
        public CandidatePastTeachingPositionProfile()
        {
            CreateMap<Entity, CandidatePastTeachingPosition>()
                .ForMember(dest => dest.SubjectTaughtId,
                    opt => opt.MapFrom(src =>
                        src.GetAttributeValue<EntityReference>("dfe_subjecttaught").Id))
                .ForMember(dest => dest.EducationPhaseId,
                    opt => opt.MapFrom(src => src.GetAttributeValue<OptionSetValue>("dfe_educationphase").Value));
        }
    }
}
