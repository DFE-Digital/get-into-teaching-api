using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Models;

namespace GetIntoTeachingApi.Services
{
    public class CrmService : ICrmService
    {
        private readonly IOrganizationServiceContextAdapter _context;
        private readonly IMapper _mapper;

        public CrmService(IOrganizationServiceContextAdapter context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TypeEntity>> GetTeachingSubjects()
        {
            return from subject in await _context.CreateQuery(ConnectionString(), "dfe_teachingsubjectlist")
                   select _mapper.Map<TypeEntity>(subject);
        }

        public async Task<IEnumerable<TypeEntity>> GetCountries()
        {
            return from subject in await _context.CreateQuery(ConnectionString(), "dfe_country")
                   select _mapper.Map<TypeEntity>(subject);
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
