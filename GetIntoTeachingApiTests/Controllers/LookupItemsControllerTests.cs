using GetIntoTeachingApi.Controllers;
using GetIntoTeachingApiTests.Helpers;
using FluentAssertions;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using Microsoft.AspNetCore.Authorization;
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
        public async void GetCountries_ReturnsAllCountriesSortedByCountryName()
        {
            var mockCountries = MockCountries();
            _mockStore.Setup(mock => mock.GetCountries()).Returns(mockCountries.AsAsyncQueryable());

            var response = await _controller.GetCountries();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            var countries = (IEnumerable<Country>)ok.Value;
            countries.Select(c => c.Value).Should().BeEquivalentTo(new[] { "Item 1", "Item 2", "Item 3" });
        }

        [Fact]
        public async void GetTeachingSubjects_ReturnsAllSubjectsSortedBySubjectName()
        {
            var mockSubjects = MockTeachingSubjects();
            _mockStore.Setup(mock => mock.GetTeachingSubjects()).Returns(mockSubjects.AsAsyncQueryable());

            var response = await _controller.GetTeachingSubjects();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            var subjects = (IEnumerable<TeachingSubject>)ok.Value;
            subjects.Select(c => c.Value).Should().BeEquivalentTo(new[] { "Item 1", "Item 2", "Item 3" });
        }

        private static TeachingSubject[] MockTeachingSubjects()
        {
            return new[]
            {
                new TeachingSubject {Id = Guid.NewGuid(), Value = "Item 2"},
                new TeachingSubject {Id = Guid.NewGuid(), Value = "Item 3"},
                new TeachingSubject {Id = Guid.NewGuid(), Value = "Item 1"},
            };
        }

        private static Country[] MockCountries()
        {
            return new[]
            {
                new Country {Id = Guid.NewGuid(), Value = "Item 2"},
                new Country {Id = Guid.NewGuid(), Value = "Item 3"},
                new Country {Id = Guid.NewGuid(), Value = "Item 1"},
            };
        }
    }
}
