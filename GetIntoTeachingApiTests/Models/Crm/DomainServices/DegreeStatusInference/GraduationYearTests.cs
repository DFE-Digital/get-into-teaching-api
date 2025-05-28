using GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices;
using GetIntoTeachingApiTests.Models.Crm.DomainServices.DegreeStatusInference.TestDoubles;
using System;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Crm.DomainServices.DegreeStatusInference
{
    public sealed class GraduationYearTests
    {
        private readonly GraduationYear _graduationYear;
        private readonly ICurrentYearProvider _currentYearProvider;

        public GraduationYearTests()
        {
            ICurrentYearProvider currentYearProvider =
                CurrentYearProviderTestDouble.StubFor(new DateTime(2021, 01, 01));

            _currentYearProvider = currentYearProvider;

            _graduationYear = new GraduationYear(2021, currentYearProvider);
        }

        [Fact]
        public void GetProposedGraduationEndDate_ValidYearProvided_ReturnsConfiguredProposedGraduationEndDate()
        {
            // act
            DateTimeOffset result = _graduationYear.GetProposedGraduationEndDate();

            // assert
            Assert.Equal(2021, result.Year);    // 2021
            Assert.Equal(08, result.Month);     // August
            Assert.Equal(31, result.Day);       // 31st
        }

        [Fact]
        public void GetProposedGraduationStartDate_ValidYearProvided_ReturnsConfiguredProposedGraduationStartDate()
        {
            // act
            DateTimeOffset result = _graduationYear.GetProposedGraduationStartDate();

            // assert
            Assert.Equal(2020, result.Year);    // 2020
            Assert.Equal(09, result.Month);     // September
            Assert.Equal(01, result.Day);       // 1st
        }
    }
}
