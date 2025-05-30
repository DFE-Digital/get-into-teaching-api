using System;
using System.Linq;
using FluentAssertions;
using FluentValidation.TestHelper;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.Crm.Validators;
using GetIntoTeachingApi.Services;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Crm.Validators
{
    public class ContactChannelCreationValidatorTests
    {
        private readonly ContactChannelCreationValidator _validator;
        private readonly Mock<IStore> _mockStore;

        public ContactChannelCreationValidatorTests()
        {
            _mockStore = new Mock<IStore>();
            _validator = new ContactChannelCreationValidator(_mockStore.Object);
        }

        [Fact]
        public void Validate_WhenValid_HasNoErrors()
        {
            var mockPickListItem = new PickListItem { Id = 123 };

            _mockStore
                .Setup(mock => mock.GetPickListItems("dfe_contactchannelcreation", "dfe_creationchannelsource"))
                .Returns(new[] { mockPickListItem }.AsQueryable());
            
            _mockStore
                .Setup(mock => mock.GetPickListItems("dfe_contactchannelcreation", "dfe_creationchannelservice"))
                .Returns(new[] { mockPickListItem }.AsQueryable());

            _mockStore
                .Setup(mock => mock.GetPickListItems("dfe_contactchannelcreation", "dfe_creationchannelactivities"))
                .Returns(new[] { mockPickListItem }.AsQueryable());

            var contactChannelCreation = new ContactChannelCreation()
            {
                CreationChannelSourceId = mockPickListItem.Id,
                CreationChannelServiceId = mockPickListItem.Id,
                CreationChannelActivityId = mockPickListItem.Id,
            };

            var result = _validator.TestValidate(contactChannelCreation);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_ChannelIdIsInvalid_HasError()
        {
            var result = _validator.TestValidate(new ContactChannelCreation() { CreationChannelSourceId = 123 });
        
            result.ShouldHaveValidationErrorFor(r => r.CreationChannelSourceId);
        }
    }
}
