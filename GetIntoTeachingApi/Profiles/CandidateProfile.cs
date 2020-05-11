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
            CreateMap<Entity, Candidate>().ForMember(dest =>
                dest.Email,
                opt => opt.MapFrom(src => src.GetAttributeValue<string>("emailaddress1"))
            ).ForMember(dest =>
                dest.FirstName,
                opt => opt.MapFrom(src => src.GetAttributeValue<string>("firstname"))
            ).ForMember(dest =>
                dest.LastName,
                opt => opt.MapFrom(src => src.GetAttributeValue<string>("lastname"))
            ).ForMember(dest =>
                dest.DateOfBirth,
                opt => opt.MapFrom(src => src.GetAttributeValue<DateTime>("birthdate"))
            );
        }
    }
}
