using System.Collections.Generic;
using FluentAssertions;
using GetIntoTeachingApi.Controllers;
using GetIntoTeachingApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Xunit;

namespace GetIntoTeachingApiTests.Controllers
{
    public class OperationsControllerTests
    {
        [Fact]
        public void GenerateMappingInfo_RespondsWithMappingInfo()
        {
            var controller = new OperationsController();

            var response = controller.GenerateMappingInfo();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            var mappings = ok.Value.Should().BeOfType<List<MappingInfo>>().Subject;
            mappings.Count().Should().Be(10);
            mappings.Any(m => m.LogicalName == "contact" &&
                              m.Class == "GetIntoTeachingApi.Models.Candidate"
                              ).Should().BeTrue();
        }
    }
}