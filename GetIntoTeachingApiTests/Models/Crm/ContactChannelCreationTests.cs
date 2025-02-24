using FluentAssertions;
using GetIntoTeachingApi.Attributes;
using GetIntoTeachingApi.Models.Crm;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Crm
{
    public class ContactChannelCreationTests
    {
        [Fact]
        public void EntityAttributes()
        {
            var type = typeof(ContactChannelCreation);

            type.Should().BeDecoratedWith<EntityAttribute>(a => a.LogicalName == "dfe_contactchannelcreation");
            type.Should().BeDecoratedWith<SwaggerIgnoreAttribute>();

            type.GetProperty("CreationChannelSourceId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_creationchannelsource" && a.Type == typeof(OptionSetValue));
            type.GetProperty("CreationChannelServiceId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_creationchannelservice" && a.Type == typeof(OptionSetValue));
            type.GetProperty("CreationChannelActivityId").Should().BeDecoratedWith<EntityFieldAttribute>(
                a => a.Name == "dfe_creationchannelactivities" && a.Type == typeof(OptionSetValue));

            type.GetProperty("CreatedBy").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "createdby");
            type.GetProperty("CandidateId").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_contactid");
            type.GetProperty("CreationChannel").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_creationchannel");
            type.GetProperty("CreationChannelSourceId").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_creationchannelsource");
            type.GetProperty("CreationChannelServiceId").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_creationchannelservice");
            type.GetProperty("CreationChannelActivityId").Should().BeDecoratedWith<EntityFieldAttribute>(a => a.Name == "dfe_creationchannelactivities");
        }

        [Fact]
        public void CreationChannel_DefaultValue_IsCorrect()
        {
            new ContactChannelCreation().CreationChannel.Should().BeFalse();
        }
    }
}