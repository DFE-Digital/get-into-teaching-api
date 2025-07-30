using FluentAssertions;
using GetIntoTeachingApi.Jobs.CandidateSanitisation.ContactChannelCreationModelSanitisation;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApiTests.Jobs.CandidateSanitisation.TestDoubles;
using System;
using System.Collections.Generic;
using Xunit;

namespace GetIntoTeachingApiTests.Jobs.CrmModelSanitisation.ContactChannelCreationModelSanitisation;

public class ContactChannelCreationDuplicateSanitisationRuleTests
{
    [Fact]
    public void SanitiseCrmModel_WithNullWrapper_ThrowsArgumentNullException()
    {
        // arrange
        ContactChannelCreationDuplicateSanitisationRule rule = new();

        // assert
        Assert.Throws<ArgumentNullException>(() => rule.SanitiseCrmModel(null));
    }

    [Fact]
    public void SanitiseCrmModel_WithMatchingCreationChannel_RemovesCreationChannel()
    {
        // arrange
        ContactChannelCreation creationChannel =
            ContactChannelCreationTestDouble.BuildSingleContactChannel(
                ContactChannelCreation.CreationChannelSource.SchoolExperience,
                ContactChannelCreation.CreationChannelService.CreatedOnSchoolExperience);

        List<ContactChannelCreation> candidateContactChannelCreations = ContactChannelCreationTestDouble.BuildDefaultContactCreationChannelsStub();
        ContactChannelCreationSanitisationRequestWrapper wrapper = new(creationChannel, candidateContactChannelCreations.AsReadOnly());
        ContactChannelCreationDuplicateSanitisationRule rule = new();

        // act
        rule.SanitiseCrmModel(wrapper);

        // assert
        wrapper.Preserve.Should().BeFalse();
        wrapper.CreationChannel.Should().Be(null);
    }

    [Fact]
    public void SanitiseCrmModel_MismatchedCreationChannel_PreservesCreationChannel()
    {
        // arrange
        ContactChannelCreation creationChannel =
            ContactChannelCreationTestDouble.BuildSingleContactChannel(
                ContactChannelCreation.CreationChannelSource.CheckinApp,
                ContactChannelCreation.CreationChannelService.PaidSearch);

        List<ContactChannelCreation> candidateContactChannelCreations = ContactChannelCreationTestDouble.BuildDefaultContactCreationChannelsStub();
        ContactChannelCreationSanitisationRequestWrapper wrapper = new(creationChannel, candidateContactChannelCreations.AsReadOnly());
        ContactChannelCreationDuplicateSanitisationRule rule = new();

        // act
        rule.SanitiseCrmModel(wrapper);

        // assert
        wrapper.Preserve.Should().BeTrue();
        wrapper.CreationChannel.Should().Be(creationChannel);
    }
}