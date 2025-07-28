using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApiTests.Jobs.CandidateSanitisation.TestDoubles;
using GetIntoTeachingApi.Jobs.CandidateSanitisation;
using GetIntoTeachingApi.Jobs.CandidateSanitisation.ContactChannelCreationModelSanitisation;
using GetIntoTeachingApi.Models.Crm;
using Moq;


namespace GetIntoTeachingApiTests.Jobs.CrmModelSanitisation.ContactChannelCreationModelSanitisation
{
    public class ContactChannelCreationSanitisationRulesHandlerTests
    {
        // Tests to implement:
        // throws ArgumentNullException if wrapper is null
        // calls SanitiseCrmModel for each sanitisationRule 

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
            List<ICrmModelSanitisationRule<ContactChannelCreationSanitisationRequestWrapper>> rules = new();

            Mock<ICrmModelSanitisationRule<ContactChannelCreationSanitisationRequestWrapper>> mockRule1 = new();
            Mock<ICrmModelSanitisationRule<ContactChannelCreationSanitisationRequestWrapper>> mockRule2 = new();


            rules.Add(mockRule1.Object);
            rules.Add(mockRule2.Object);

            ContactChannelCreationSanitisationRulesHandler rulesHandler = new(rules);

            // act
            rulesHandler.SanitiseCrmModelWithRules(wrapper);

            // assert
            mockRule1.Verify(mock => mock.SanitiseCrmModel(wrapper), Times.Once);
            mockRule2.Verify(mock => mock.SanitiseCrmModel(wrapper), Times.Once);
        }
    }
}
