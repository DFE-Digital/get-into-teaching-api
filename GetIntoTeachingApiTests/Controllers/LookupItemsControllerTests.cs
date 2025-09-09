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
using System.Threading.Tasks;
using Bogus;

namespace GetIntoTeachingApiTests.Controllers
{
    public class LookupItemsControllerTests
    {
        private static readonly Faker _faker = new Faker();
        private readonly Mock<IStore> _mockStore;
        private readonly LookupItemsController _controller;

        private const int MaxCountries = 300;
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
        public async Task GetCountries_ReturnsAllCountriesSortedByCountryName()
        {
            var testCountries = FakeCountries();
            _mockStore.Setup(mock => mock.GetCountries()).Returns(testCountries.AsAsyncQueryable());

            var response = await _controller.GetCountries();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            var countries = (IEnumerable<Country>)ok.Value;
            countries.Should().BeEquivalentTo(testCountries.AsEnumerable());
        }

        [Fact]
        public async Task GetTeachingSubjects_ReturnsAllSubjectsSortedBySubjectName()
        {
            var mockSubjects = MockTeachingSubjects();
            _mockStore.Setup(mock => mock.GetTeachingSubjects()).Returns(mockSubjects.AsAsyncQueryable());

            var response = await _controller.GetTeachingSubjects();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            var subjects = (IEnumerable<TeachingSubject>)ok.Value;
            subjects.Select(c => c.Value).Should().BeEquivalentTo(new[] { "Item 1", "Item 2", "Item 3" });
        }

        [Fact]
        public async Task GetTeachingSubjects_ReturnsAllDegreeCountries()
        {
            var mockCountries = FakeCountries();
            foreach (var countryID in Country.DegreeCountriesList)
            {
                Country fakeCountry = FakeCountry();
                fakeCountry.Id = countryID;
                mockCountries.Add(fakeCountry);
            }
            
            _mockStore.Setup(mock => mock.GetCountries()).Returns(mockCountries.AsAsyncQueryable());

            IActionResult result = await _controller.GetDegreeCountries();

            OkObjectResult objectResult = result.Should().BeOfType<OkObjectResult>().Subject;
            IEnumerable<Country> countriesList = (IEnumerable<Country>)objectResult.Value;
            countriesList?.Count().Should().BeLessThan(MaxCountries);
            countriesList?.Count().Should().Be(Country.DegreeCountriesList.Count);
            countriesList?.Select(c => c.Id).Should().BeEquivalentTo(Country.DegreeCountriesList.AsEnumerable());
            _mockStore.Verify(s => s.GetCountries(), Times.Once);
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

        private static Faker<Country> FakeCountry() =>
            new Faker<Country>()
                .RuleFor(c => c.Id, f => _faker.Random.Guid())
                .RuleFor(c => c.Value, f => _faker.Address.Country())
                .RuleFor(c => c.IsoCode, f => _faker.Address.CountryCode());

        private static List<Country> FakeCountries() =>
            FakeCountry().Generate(_faker.Random.Number(3,MaxCountries));
        
        // private static Country[] MockCountries() => new[]
        //     {
        //         new Country {Id = Guid.NewGuid(), Value = "Item 2"},
        //         new Country {Id = Guid.NewGuid(), Value = "Item 3"},
        //         new Country {Id = Guid.NewGuid(), Value = "Item 1"},
        //     };
        
    }
}
