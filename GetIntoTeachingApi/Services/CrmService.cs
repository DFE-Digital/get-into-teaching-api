using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Models;
using Microsoft.Xrm.Sdk;

namespace GetIntoTeachingApi.Services
{
    public class CrmService : ICrmService
    {       
        public enum PrivacyPolicyType { Web = 222750001 }
        private readonly IOrganizationServiceContextAdapter _context;
        private readonly IMapper _mapper;

        public CrmService(IOrganizationServiceContextAdapter context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TypeEntity>> GetTeachingSubjects()
        {
            return (await _context.CreateQuery(ConnectionString(), "dfe_teachingsubjectlist"))
                .Select((subject) => _mapper.Map<TypeEntity>(subject));
        }

        public async Task<IEnumerable<TypeEntity>> GetCountries()
        {
            return (await _context.CreateQuery(ConnectionString(), "dfe_country"))
                .Select((subject) => _mapper.Map<TypeEntity>(subject));
        }

        public async Task<PrivacyPolicy> GetLatestPrivacyPolicy()
        {
            return (await _context.CreateQuery(ConnectionString(), "dfe_privacypolicy"))
                .Where((policy) => 
                    policy.GetAttributeValue<OptionSetValue>("dfe_policytype").Value == (int) PrivacyPolicyType.Web && 
                    policy.GetAttributeValue<bool>("dfe_active")
                )
                .OrderByDescending((policy) => policy.GetAttributeValue<DateTime>("createdon"))
                .Select((policy) => _mapper.Map<PrivacyPolicy>(policy))
                .First();
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
