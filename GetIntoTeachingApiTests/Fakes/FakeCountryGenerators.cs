using System.Collections.Generic;
using Bogus;
using GetIntoTeachingApi.Models;

namespace GetIntoTeachingApiTests.Fakes;

public class FakeCountryGenerators
{
    private static Faker _faker = new Faker();
    
    public const int MaxCountries = 300;

    public static Faker<Country> FakeCountry() =>
        new Faker<Country>()
            .RuleFor(c => c.Id, f => _faker.Random.Guid())
            .RuleFor(c => c.Value, f => _faker.Address.Country())
            .RuleFor(c => c.IsoCode, f => _faker.Address.CountryCode());

    public static List<Country> FakeCountries() =>
        FakeCountry().Generate(_faker.Random.Number(3,MaxCountries));

    public static List<Country> FakeCountriesWithDegreeCountries()
    {
        List<Country> countries = FakeCountries();
        foreach (var countryID in Country.DegreeCountriesList) 
        {
            Country fakeCountry = FakeCountry();
            fakeCountry.Id = countryID; 
            countries.Add(fakeCountry);
        }
        return countries;
    } 

}