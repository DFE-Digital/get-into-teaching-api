using GetIntoTeachingApi.Jobs.CandidateSanitisation;
using GetIntoTeachingApi.Jobs.CandidateSanitisation.ContactChannelCreationModelSanitisation;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApiTests.Jobs.CandidateSanitisation.TestDoubles;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace GetIntoTeachingApiTests.Jobs.CrmModelSanitisation.ContactChannelCreationModelSanitisation;

public class ContactChannelCreationSanitisationRulesHandlerTests
{
    [Fact]
    public void SanitiseCrmModelWithRules_WithRules_CallsSanitiseCrmModelForEachRule()
    {
        // arrange
        ContactChannelCreation creationChannel = ContactChannelCreationTestDouble.BuildSingleContactChannel(
            ContactChannelCreation.CreationChannelSource.SchoolExperience,
            ContactChannelCreation.CreationChannelService.CreatedOnSchoolExperience);
        List<ContactChannelCreation> candidateContactChannelCreations =
            ContactChannelCreationTestDouble.BuildDefaultContactCreationChannelsStub();
        ContactChannelCreationSanitisationRequestWrapper wrapper = new(creationChannel,
            candidateContactChannelCreations.AsReadOnly());

        List<ICrmModelSanitisationRule<ContactChannelCreationSanitisationRequestWrapper>> rules = [];

        Mock<ICrmModelSanitisationRule<ContactChannelCreationSanitisationRequestWrapper>> mockRule1 = new();
        mockRule1
            .Setup(mock => mock.SanitiseCrmModel(It.IsAny<ContactChannelCreationSanitisationRequestWrapper>()))
            .Returns((ContactChannelCreationSanitisationRequestWrapper wrapper) => wrapper);

        Mock<ICrmModelSanitisationRule<ContactChannelCreationSanitisationRequestWrapper>> mockRule2 = new();
        mockRule2
            .Setup(mock => mock.SanitiseCrmModel(It.IsAny<ContactChannelCreationSanitisationRequestWrapper>()))
            .Returns((ContactChannelCreationSanitisationRequestWrapper wrapper) => wrapper);

        rules.Add(mockRule1.Object);
        rules.Add(mockRule2.Object);

        ContactChannelCreationSanitisationRulesHandler rulesHandler = new(rules);

        // act
        rulesHandler.SanitiseCrmModelWithRules(wrapper);

        // assert
        mockRule1.Verify(mock => mock.SanitiseCrmModel(wrapper), Times.Once);
        mockRule2.Verify(mock => mock.SanitiseCrmModel(wrapper), Times.Once);
    }

    [Fact]
    public void SanitiseCrmModelWithRules_NullWrapper_ThrowsArgumentNullException()
    {
        // arrange
        List<ICrmModelSanitisationRule<ContactChannelCreationSanitisationRequestWrapper>> rules = [];
        ContactChannelCreationSanitisationRulesHandler rulesHandler = new(rules);
        // act & assert
        Assert.Throws<ArgumentNullException>(() => rulesHandler.SanitiseCrmModelWithRules(null));
    }
}
