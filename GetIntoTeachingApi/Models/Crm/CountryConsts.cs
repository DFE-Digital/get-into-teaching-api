using System;
using System.Collections.Generic;

namespace GetIntoTeachingApi.Models.Crm;

public class CountryConsts
{
    public static readonly Guid UnitedKingdom = new Guid("72f5c2e6-74f9-e811-a97a-000d3a2760f2");
    public static readonly Guid AnotherCountry = new Guid("6f9e7b81-e44d-f011-877a-00224886d23e");

    public static readonly List<Guid?> ValidDegreeCountries = new List<Guid?>() { UnitedKingdom, AnotherCountry };
}