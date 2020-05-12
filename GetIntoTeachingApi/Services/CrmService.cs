using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Models;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Services
{
    public class CrmService : ICrmService
    {       
        public enum PrivacyPolicyType { Web = 222750001 }
        private readonly IOrganizationServiceAdapter _organizationalService;
        private readonly IMapper _mapper;

        public CrmService(IOrganizationServiceAdapter organizationalService, IMapper mapper)
        {
            _organizationalService = organizationalService;
            _mapper = mapper;
        }

        public IEnumerable<TypeEntity> GetTeachingSubjects()
        {
            return _organizationalService.CreateQuery(ConnectionString(), "dfe_teachingsubjectlist")
                .Select((subject) => _mapper.Map<TypeEntity>(subject));
        }

        public IEnumerable<TypeEntity> GetCountries()
        {
            return _organizationalService.CreateQuery(ConnectionString(), "dfe_country")
                .Select((subject) => _mapper.Map<TypeEntity>(subject));
        }

        public PrivacyPolicy GetLatestPrivacyPolicy()
        {
            return _organizationalService.CreateQuery(ConnectionString(), "dfe_privacypolicy")
                .Where((policy) => 
                    policy.GetAttributeValue<OptionSetValue>("dfe_policytype").Value == (int) PrivacyPolicyType.Web && 
                    policy.GetAttributeValue<bool>("dfe_active")
                )
                .OrderByDescending((policy) => policy.GetAttributeValue<DateTime>("createdon"))
                .Select((policy) => _mapper.Map<PrivacyPolicy>(policy))
                .First();
        }

        public Candidate GetCandidate(string email)
        {
            return _organizationalService.CreateQuery(ConnectionString(), "contact")
                .Where((contact) =>
                    // Will perform a case-insensitive comparison
                    contact.GetAttributeValue<string>("emailaddress1") == email
                )
                .OrderByDescending((contact) => contact.GetAttributeValue<DateTime>("createdon"))
                .Select((candidate) => _mapper.Map<Candidate>(candidate))
                .FirstOrDefault();
        }


        private string ConnectionString()
        {
            return $"AuthType=ClientSecret; url={InstanceUrl()}; ClientId={ClientId()}; ClientSecret={ClientSecret()}";
        }

        private string InstanceUrl()
        {
            return Environment.GetEnvironmentVariable("CRM_SERVICE_URL");
        }

        private string ClientId()
        {
            return Environment.GetEnvironmentVariable("CRM_CLIENT_ID");
        }

        private string ClientSecret()
        {
            return Environment.GetEnvironmentVariable("CRM_CLIENT_SECRET");
        }
    }
}
