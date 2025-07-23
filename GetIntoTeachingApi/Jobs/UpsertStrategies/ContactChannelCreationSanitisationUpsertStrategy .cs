using GetIntoTeachingApi.Jobs.CandidateSanitisation;
using GetIntoTeachingApi.Jobs.CandidateSanitisation.ContactChannelCreationModelSanitisation;
using GetIntoTeachingApi.Jobs.CrmModelSanitisation.ContactChannelCreationModelSanitisation.Repositories;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Services;
using System.Collections.Generic;
using System.Linq;

namespace GetIntoTeachingApi.Jobs.UpsertStrategies
{
    /// <summary>
    /// Implements upsert logic for ContactChannelCreation entities.
    /// Combines candidate context with sanitisation rules to ensure safe persistence.
    /// </summary>
    public class ContactChannelCreationSanitisationUpsertStrategy : ICrmlUpsertStrategy<ContactChannelCreation>
    {
        private readonly ICrmService _crmService;
        private readonly ICrmModelSanitisationRulesHandler<ContactChannelCreationSanitisationRequestWrapper> _rulesHandler;
        private readonly ICandidateContactChannelCreationsRepository _candidateContactChannelCreationsRepository;

        /// <summary>
        /// Constructs the upsert strategy with domain service dependencies.
        /// </summary>
        public ContactChannelCreationSanitisationUpsertStrategy(
            ICrmService crmService,
            ICrmModelSanitisationRulesHandler<ContactChannelCreationSanitisationRequestWrapper> rulesHandler,
            ICandidateContactChannelCreationsRepository candidateContactChannelCreationsRepository)
        {
            _crmService = crmService;
            _rulesHandler = rulesHandler;
            _candidateContactChannelCreationsRepository = candidateContactChannelCreationsRepository;
        }

        /// <summary>
        /// Attempts to upsert a contact channel creation model.
        /// Applies sanitisation rules before determining persistence outcome.
        /// </summary>
        /// <param name="model">The contact channel creation entity to evaluate and potentially persist.</param>
        /// <param name="logMessage">Descriptive outcome message for diagnostic or audit logging.</param>
        /// <returns>True if the model is persisted; false if discarded due to duplication or invalid state.</returns>
        public bool TryUpsert(ContactChannelCreation model, out string logMessage)
        {
            // Retrieve existing channel creations for the candidate.
            IEnumerable<ContactChannelCreation> contactChannelCreations =
                _candidateContactChannelCreationsRepository.GetContactChannelCreationsByCandidateId(model.CandidateId);

            // Package incoming and historical data into a sanitisation wrapper.
            ContactChannelCreationSanitisationRequestWrapper wrapper =
                ContactChannelCreationSanitisationRequestWrapper.Create(
                    model, contactChannelCreations.ToList().AsReadOnly());

            // Apply rule-based sanitisation logic.
            wrapper = _rulesHandler.SanitiseCrmModelWithRules(wrapper);

            // Persist only if the wrapper signals preservation eligibility.
            if (wrapper.Preserve)
            {
                _crmService.Save(model);

                logMessage = $"Saved model: {model.Id}";

                SaveResult saveResult =
                    _candidateContactChannelCreationsRepository
                        .SaveContactChannelCreations(
                            ContactChannelCreationSaveRequest.Create(
                            model.CandidateId, model, contactChannelCreations.ToList().AsReadOnly()));

                logMessage = saveResult.Message;
                return saveResult.IsSuccessful;
            }

            logMessage = $"Model not preserved: {model.Id}";
            return false;
        }
    }
}
