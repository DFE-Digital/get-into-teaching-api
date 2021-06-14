using System;

namespace GetIntoTeachingApi.Models
{
    public interface IHasCandidateId
    {
        public Guid CandidateId { get; }
    }
}
