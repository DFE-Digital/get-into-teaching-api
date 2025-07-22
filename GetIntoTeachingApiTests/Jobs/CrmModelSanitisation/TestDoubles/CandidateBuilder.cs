using GetIntoTeachingApi.Models.Crm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GetIntoTeachingApiTests.Jobs.CandidateSanitisation.TestDoubles;

/// <summary>
/// A test utility for fluently constructing <see cref="Candidate"/> objects in unit tests.
/// </summary>
internal class CandidateBuilder
{
    private readonly Candidate _candidate = new();

    /// <summary>
    /// Assigns an ID to the candidate.
    /// </summary>
    /// <param name="id">The <see cref="Guid"/> to assign.</param>
    /// <returns>The current <see cref="CandidateBuilder"/> instance.</returns>
    public CandidateBuilder WithId(Guid id)
    {
        _candidate.Id = id;
        return this;
    }

    /// <summary>
    /// Adds a collection of contact channel creation records to the candidate.
    /// </summary>
    /// <param name="channels">The contact channel creations to add.</param>
    /// <returns>The current <see cref="CandidateBuilder"/> instance.</returns>
    public CandidateBuilder WithContactChannels(IEnumerable<ContactChannelCreation> channels)
    {
        channels.ToList().ForEach(channel =>
            _candidate.ContactChannelCreations.Add(channel));

        return this;
    }

    /// <summary>
    /// Adds a single contact channel creation to the candidate.
    /// </summary>
    /// <param name="channel">The <see cref="ContactChannelCreation"/> to add.</param>
    /// <returns>The current <see cref="CandidateBuilder"/> instance.</returns>
    public CandidateBuilder WithContactChannel(ContactChannelCreation channel)
    {
        _candidate.ContactChannelCreations.Add(channel);
        return this;
    }

    /// <summary>
    /// Adds a default set of contact channel creation records using the standard test stub.
    /// </summary>
    /// <returns>The current <see cref="CandidateBuilder"/> instance.</returns>
    public CandidateBuilder WithDefaultChannels()
    {
        _candidate.ContactChannelCreations.AddRange(
            ContactChannelCreationTestDouble.BuildDefaultContactCreationChannelsStub());

        return this;
    }

    /// <summary>
    /// Finalizes and returns the constructed <see cref="Candidate"/> object.
    /// </summary>
    /// <returns>A fully built <see cref="Candidate"/> instance.</returns>
    public Candidate Build() => _candidate;
}