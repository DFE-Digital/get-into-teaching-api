using GetIntoTeachingApi.Models;
using System.Collections.Generic;

namespace GetIntoTeachingApi.Services
{
    public interface ICrmService
    {
        public IEnumerable<TypeEntity> GetTeachingSubjects();
        public IEnumerable<TypeEntity> GetCountries();
    }
}
