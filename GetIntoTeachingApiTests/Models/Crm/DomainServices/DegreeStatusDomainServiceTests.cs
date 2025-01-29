using GetIntoTeachingApi.Models.Crm.DomainServices;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Crm.DomainServices
{
    public sealed class DegreeStatusDomainServiceTests
    {
        [Fact]
        public void GetInferredDegreeStatusFromGraduationYear_ThrowsNotImplementedException()
        {
            // arrange
            DegreeStatusDomainService service = new();
            GraduationYear graduationYear = new(2021);

            // act
            int? degreeStatusId = service.GetInferredDegreeStatusFromGraduationYear(graduationYear);

            Assert.NotNull(degreeStatusId);
        }
    }
}
