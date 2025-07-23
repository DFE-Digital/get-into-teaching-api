using GetIntoTeachingApi.Database;
using GetIntoTeachingApi.Models.Crm;
using System;
using System.Collections.Generic;

namespace GetIntoTeachingApi.Jobs.CrmModelSanitisation.ContactChannelCreationModelSanitisation.Repositories;

public class CandidateContactChannelCreationsRepository : ICandidateContactChannelCreationsRepository
{
    private readonly GetIntoTeachingDbContext _dbContext;

    public CandidateContactChannelCreationsRepository(GetIntoTeachingDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public IEnumerable<ContactChannelCreation> GetContactChannelCreationsByCandidateId(Guid candidateId)
    {
        throw new NotImplementedException();
    }

    public SaveResult SaveContactChannelCreations(ContactChannelCreationSaveRequest saveRequest)
    {
        throw new NotImplementedException();
    }
}