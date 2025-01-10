using GetIntoTeaching.Application.Repositories;
using GetIntoTeaching.Core.Application.UseCase;
using GetIntoTeaching.Domain;

namespace GetIntoTeaching.Application.UseCases.TeacherTrainingAdviser
{
    public sealed class AssignCandidateToTrainingAdviserUseCase : IUseCase<object, UseCaseResponse<Candidate>>
    {
        public readonly ICandidateReadOnlyRepository _candidateReadOnlyRepository;

        public AssignCandidateToTrainingAdviserUseCase(ICandidateReadOnlyRepository candidateReadOnlyRepository)
        {
            _candidateReadOnlyRepository = candidateReadOnlyRepository;
        }

        public async Task<UseCaseResponse<Candidate>> HandleRequest(object request)
        {
            Candidate candidate =
                await _candidateReadOnlyRepository.GetCandidateByIdentifiert(Guid.NewGuid());

            return UseCaseResponse<Candidate>.Success(candidate);
        }
    }
}
