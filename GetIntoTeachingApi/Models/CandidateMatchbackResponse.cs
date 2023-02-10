using System;
using GetIntoTeachingApi.Models.Crm;

namespace GetIntoTeachingApi.Models
{
	public class CandidateMatchbackResponse
	{
		public Guid CandidateId { get; set; }

		public CandidateMatchbackResponse(Candidate candidate)
		{
            CandidateId = (Guid) candidate.Id;
		}
	}
}

