using GetIntoTeaching.Core.Domain;

namespace GetIntoTeaching.Domain.Services
{
    public interface ICandidateRegistrationService : IDomainService
    {
        bool IsCandidateAssignedToTrainingAdviser(Guid candidateId);
    }
}
