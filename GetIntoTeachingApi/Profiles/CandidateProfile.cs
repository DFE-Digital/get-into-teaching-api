using AutoMapper;
using GetIntoTeachingApi.Models;
using Microsoft.Xrm.Sdk;
using System;

namespace GetIntoTeachingApi.Profiles
{
    public class CandidateProfile : Profile
    {
        public CandidateProfile()
        {
            CreateMap<Entity, Candidate>()
                .ForMember(dest => dest.PreferredTeachingSubjectId,
                    opt => opt.MapFrom(src =>
                        src.GetAttributeValue<EntityReference>("dfe_preferredteachingsubject01").Id))
                .ForMember(dest => dest.PreferredEducationPhaseId,
                    opt => opt.MapFrom(src =>
                        src.GetAttributeValue<OptionSetValue>("dfe_preferrededucationphase01").Value))
                .ForMember(dest => dest.LocationId,
                    opt => opt.MapFrom(src => src.GetAttributeValue<OptionSetValue>("dfe_isinuk").Value))
                .ForMember(dest => dest.InitialTeacherTrainingYearId,
                    opt => opt.MapFrom(src => src.GetAttributeValue<OptionSetValue>("dfe_ittyear").Value))
                .ForMember(dest => dest.Email,
                    opt => opt.MapFrom(src => src.GetAttributeValue<string>("emailaddress1")))
                .ForMember(dest => dest.FirstName,
                    opt => opt.MapFrom(src => src.GetAttributeValue<string>("firstname")))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.GetAttributeValue<string>("lastname")))
                .ForMember(dest => dest.DateOfBirth,
                    opt => opt.MapFrom(src => src.GetAttributeValue<DateTime>("birthdate")))
                .ForPath(dest => dest.Address.Line1,
                    opt => opt.MapFrom(src => src.GetAttributeValue<string>("address1_line1")))
                .ForPath(dest => dest.Address.Line2,
                    opt => opt.MapFrom(src => src.GetAttributeValue<string>("address1_line2")))
                .ForPath(dest => dest.Address.Line3,
                    opt => opt.MapFrom(src => src.GetAttributeValue<string>("address1_line3")))
                .ForPath(dest => dest.Address.City,
                    opt => opt.MapFrom(src => src.GetAttributeValue<string>("address1_city")))
                .ForPath(dest => dest.Address.State,
                    opt => opt.MapFrom(src => src.GetAttributeValue<string>("address1_stateorprovince")))
                .ForPath(dest => dest.Address.Postcode,
                    opt => opt.MapFrom(src => src.GetAttributeValue<string>("address1_postalcode")));
        }
    }
}
