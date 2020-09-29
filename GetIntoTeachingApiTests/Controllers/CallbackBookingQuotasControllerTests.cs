using GetIntoTeachingApi.Controllers;
using FluentAssertions;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using Microsoft.AspNetCore.Authorization;
using Xunit;
using GetIntoTeachingApi.Attributes;

namespace GetIntoTeachingApiTests.Controllers
{
    public class CallbackBookingQuotasControllerTests
    {
        private readonly Mock<ICrmService> _mockCrm;
        private readonly CallbackBookingQuotasController _controller;

        public CallbackBookingQuotasControllerTests()
        {
            _mockCrm = new Mock<ICrmService>();
            _controller = new CallbackBookingQuotasController(_mockCrm.Object);
        }

        [Fact]
        public void Authorize_IsPresent()
        {
            typeof(CallbackBookingQuotasController).Should().BeDecoratedWith<AuthorizeAttribute>();
        }

        [Fact]
        public void LogRequests_IsPresent()
        {
            typeof(CallbackBookingQuotasController).Should().BeDecoratedWith<LogRequestsAttribute>();
        }

        [Fact]
        public void GetAll_ReturnsAllQuotas()
        {
            var mockQuotas = new[] { MockQuota(), MockQuota() };
            _mockCrm.Setup(mock => mock.GetCallbackBookingQuotas()).Returns(mockQuotas);

            var response = _controller.GetAll();

            var ok = response.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(mockQuotas);
        }

        private static CallbackBookingQuota MockQuota()
        {
            return new CallbackBookingQuota() { Id = Guid.NewGuid(), StartAt = DateTime.UtcNow, NumberOfBookings = 4 };
        }
    }
}