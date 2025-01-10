using GetIntoTeaching.Application.Repositories;
using GetIntoTeaching.Core.Application.UseCase;
using GetIntoTeaching.Domain;

namespace GetIntoTeaching.Application.UseCases.TeacherTrainingAdviser
{
    public sealed class AssignCandidateToTrainingAdviserUseCase : IUseCase<object, object>
    {

        public readonly ICandidateReadOnlyRepository _candidateReadOnlyRepository;
        public AssignCandidateToTrainingAdviserUseCase(ICandidateReadOnlyRepository candidateReadOnlyRepository)
        {
            _candidateReadOnlyRepository = candidateReadOnlyRepository;
        }

        public async Task<object> HandleRequest(object request)
        {
            Candidate? candidate =
                await _candidateReadOnlyRepository.GetCandidateByIdentifiert(Guid.NewGuid());

            // TODO: if no candidate found add exception to Result object
        }
    }
}
