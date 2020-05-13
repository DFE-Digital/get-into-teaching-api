using AutoMapper;
using FluentAssertions;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApiTests.Utils;
using Microsoft.Xrm.Sdk;
using System;
using Xunit;
using static Microsoft.PowerPlatform.Cds.Client.CdsServiceClient;

namespace GetIntoTeachingApiTests.Profiles
{
    public class TypeEntityProfileTests
    {
        private readonly Mapper _mapper;

        public TypeEntityProfileTests()
        {
            _mapper = MapperHelpers.CreateMapper();
        }

        [Fact]
        public void TypeEntityProfile_WithALookupItem_MapsCorrectly()
        {
            var entity = new Entity();
            entity.Id = Guid.NewGuid();
            entity.Attributes["dfe_name"] = "name";

            var typeEntity = _mapper.Map<TypeEntity>(entity);

            ((Guid) typeEntity.Id).Should().Be(entity.Id);
            (typeEntity.Value as string).Should().Be(entity.GetAttributeValue<string>("dfe_name"));
        }

        [Fact]
        public void TypeEntityProfile_WithAPickListItem_MapsCorrectly()
        {
            var pickListItem = new PickListItem();
            pickListItem.PickListItemId = 123;
            pickListItem.DisplayLabel = "name";

            var typeEntity = _mapper.Map<TypeEntity>(pickListItem);

            ((int) typeEntity.Id).Should().Be(pickListItem.PickListItemId);
            (typeEntity.Value as string).Should().Be(pickListItem.DisplayLabel);
        }
    }
}
