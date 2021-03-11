using GetIntoTeachingApi.Controllers;
using GetIntoTeachingApiTests.Helpers;
using FluentAssertions;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Reflection;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using MoreLinq;
using Xunit;
using GetIntoTeachingApi.Attributes;
using System.Collections.Generic;
using System.Linq;

namespace GetIntoTeachingApiTests.Controllers
{
    public class LookupItemsControllerTests
    {
        private readonly Mock<IStore> _mockStore;
        private readonly LookupItemsController _controller;

        public LookupItemsControllerTests()
        {
            _mockStore = new Mock<IStore>();
            _controller = new LookupItemsController(_mockStore.Object);
        }

        [Fact]
        public void Authorize_IsPresent()
        {
            typeof(LookupItemsController).Should().BeDecoratedWith<AuthorizeAttribute>();
        }

        [Fact]
        public void PrivateShortTermResponseCache_IsPresent()
        {
            typeof(LookupItemsController).Should().BeDecoratedWith<PrivateShortTermResponseCacheAttribute>();
        }

        [Fact]
        public void CrmETag_IsPresent()
        {
            JobStorage.Current = new Mock<JobStorage>().Object;

            var methods = typeof(LookupItemsController).GetMethods(
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.DeclaredOnly);

            methods.ForEach(m => m.Should().BeDecoratedWith<CrmETagAttribute>());
        }

        [Fact]
        public async void GetCountries_ReturnsAllCountriesSortedByCountryName()
        {
            var mockLookupItems = MockLookupItems();
            _mockStore.Setup(mock => mock.GetLookupItems("dfe_country")).Returns(mockLookupItems.AsAsyncQueryable());

            var response = await _controller.GetCountries();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            var countries = (IEnumerable<LookupItem>)ok.Value;
            countries.Select(c => c.Value).Should().BeEquivalentTo(new[] { "Item 1", "Item 2", "Item 3" });
        }

        [Fact]
        public async void GetTeachingSubjects_ReturnsAllSubjectsSortedBySubjectName()
        {
            var mockLookupItems = MockLookupItems();
            _mockStore.Setup(mock => mock.GetLookupItems("dfe_teachingsubjectlist")).Returns(mockLookupItems.AsAsyncQueryable());

            var response = await _controller.GetTeachingSubjects();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            var subjects = (IEnumerable<LookupItem>)ok.Value;
            subjects.Select(c => c.Value).Should().BeEquivalentTo(new[] { "Item 1", "Item 2", "Item 3" });
        }

        private static LookupItem[] MockLookupItems()
        {
            return new[]
            {
                new LookupItem {Id = Guid.NewGuid(), Value = "Item 2"},
                new LookupItem {Id = Guid.NewGuid(), Value = "Item 3"},
                new LookupItem {Id = Guid.NewGuid(), Value = "Item 1"},
            };
        }
    }
}
