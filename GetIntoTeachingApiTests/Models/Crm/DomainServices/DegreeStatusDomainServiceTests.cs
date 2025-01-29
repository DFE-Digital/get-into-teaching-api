using GetIntoTeachingApi.Models.Crm.DomainServices;
using System;
using Xunit;

namespace GetIntoTeachingApiTests.Models.Crm.DomainServices
{
    public sealed class DegreeStatusDomainServiceTests
    {
        [Fact]
        public void GetInferredDegreeStatusFromGraduationYear_ThrowsNotImplementedException()
        {
            DegreeStatusDomainService service = new();
            GraduationYear graduationYear = new();

            Assert.Throws<ArgumentNullException>(() => service.GetInferredDegreeStatusFromGraduationYear(graduationYear));
        }
    }
}
