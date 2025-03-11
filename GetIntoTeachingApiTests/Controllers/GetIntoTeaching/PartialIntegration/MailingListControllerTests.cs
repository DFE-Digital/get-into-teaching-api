using FluentAssertions;
using GetIntoTeachingApi.Controllers.GetIntoTeaching;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices;
using GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices.Common;
using GetIntoTeachingApi.Models.Crm.DegreeStatusInference.DomainServices.Evaluators;
using GetIntoTeachingApi.Models.GetIntoTeaching;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using GetIntoTeachingApiTests.Models.Crm.DomainServices.DegreeStatusInference.TestDoubles;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;
using CurrentYearProviderTestDouble = GetIntoTeachingApiTests.Models.Crm.DomainServices.DegreeStatusInference.TestDoubles;

namespace GetIntoTeachingApiTests.Controllers.GetIntoTeaching.PartialIntegration
{
    public class MailingListControllerTests
    {
        private readonly Mock<ICandidateAccessTokenService> _mockAccessTokenService;
        private readonly Mock<ICandidateMagicLinkTokenService> _mockMagicLinkTokenService;
        private readonly Mock<ICrmService> _mockCrm;
        private readonly Mock<IBackgroundJobClient> _mockJobClient;
        private readonly Mock<IDateTimeProvider> _mockDateTime;
        private readonly MailingListController _controller;
        private readonly MailingListAddMember _request;

        public MailingListControllerTests()
        {
            _mockAccessTokenService = new Mock<ICandidateAccessTokenService>();
            _mockMagicLinkTokenService = new Mock<ICandidateMagicLinkTokenService>();
            _mockDateTime = new Mock<IDateTimeProvider>();
            _mockCrm = new Mock<ICrmService>();
            _mockJobClient = new Mock<IBackgroundJobClient>();
            _controller = new MailingListController(
                _mockAccessTokenService.Object,
                _mockMagicLinkTokenService.Object,
                _mockCrm.Object,
                _mockJobClient.Object,
                _mockDateTime.Object,
                degreeStatusDomainService: SetupDegreeStatusDomainServiceFixture(), // use the actual implementation.
                CurrentYearProviderTestDouble.CurrentYearProviderTestDouble.StubFor(new DateTime(2025, 01, 01))
            );

            _request = new()
            {
                Email = "test@test.com",
                FirstName = "John",
                LastName = "Doe"
            };

            // Freeze time.
            _mockDateTime.Setup(dateTimeProvider =>
                dateTimeProvider.UtcNow).Returns(DateTime.UtcNow);
        }

        [Fact]
        public void AddMember_ValidRequestWithNoGraduationYearAndDegreeStatusSpecified_VerifyDegreeInferenceBypassedAndSubmittedDegreeStatusAccepted()
        {
            // arrange
            _request.DegreeStatusId = (int)DegreeStatus.SecondYear;

            // act
            IActionResult response = _controller.AddMember(_request);

            // assert
            response.Should().BeOfType<NoContentResult>();

            _mockJobClient.Verify(backgroundJobClient =>
                backgroundJobClient.Create(
                    It.Is<Job>(
                        job => job.Type == typeof(UpsertCandidateJob) && job.Method.Name == "Run" &&
                        IsMatch((string)job.Args[0], null, DegreeStatus.SecondYear)
                    ),
                    It.IsAny<EnqueuedState>()));
        }

        [Fact]
        public void AddMember_ValidRequestWithCurrentYear2025AndGraduationYear2028_VerifyFirstYearOfDegreeWithCorrectGraduationDate()
        {
            // arrange
            _request.GraduationYear = 2028;

            // act
            IActionResult response = _controller.AddMember(_request);

            // assert
            response.Should().BeOfType<NoContentResult>();

            _mockJobClient.Verify(backgroundJobClient =>
                backgroundJobClient.Create(
                    It.Is<Job>(
                        job => job.Type == typeof(UpsertCandidateJob) && job.Method.Name == "Run" &&
                        IsMatch((string)job.Args[0], new DateTime(2028, 8, 31), DegreeStatus.FirstYear)
                    ),
                    It.IsAny<EnqueuedState>()));
        }

        [Fact]
        public void AddMember_ValidRequestWithCurrentYear2025AndGraduationYear2039_VerifyFirstYearOfDegreeWithCorrectGraduationDate()
        {
            // arrange
            _request.GraduationYear = 2039;

            // act
            IActionResult response = _controller.AddMember(_request);

            // assert
            response.Should().BeOfType<NoContentResult>();

            _mockJobClient.Verify(backgroundJobClient =>
                backgroundJobClient.Create(
                    It.Is<Job>(
                        job => job.Type == typeof(UpsertCandidateJob) && job.Method.Name == "Run" &&
                        IsMatch((string)job.Args[0], new DateTime(2039, 8, 31), DegreeStatus.FirstYear)
                    ),
                    It.IsAny<EnqueuedState>()));
        }

        [Fact]
        public void AddMember_ValidRequestWithCurrentYear2025AndGraduationYear2027_VerifySecondYearOfDegreeWithCorrectGraduationDate()
        {
            // arrange
            _request.GraduationYear = 2027;

            // act
            IActionResult response = _controller.AddMember(_request);

            // assert
            response.Should().BeOfType<NoContentResult>();

            _mockJobClient.Verify(backgroundJobClient =>
                backgroundJobClient.Create(
                    It.Is<Job>(
                        job => job.Type == typeof(UpsertCandidateJob) && job.Method.Name == "Run" &&
                        IsMatch((string)job.Args[0], new DateTime(2027, 8, 31), DegreeStatus.SecondYear)
                    ),
                    It.IsAny<EnqueuedState>()));
        }

        [Fact]
        public void AddMember_ValidRequestWithCurrentYear2025AndGraduationYear2026_VerifyFinalYearOfDegreeWithCorrectGraduationDate()
        {
            // arrange
            _request.GraduationYear = 2026;

            // act
            IActionResult response = _controller.AddMember(_request);

            // assert
            response.Should().BeOfType<NoContentResult>();

            _mockJobClient.Verify(backgroundJobClient =>
                backgroundJobClient.Create(
                    It.Is<Job>(
                        job => job.Type == typeof(UpsertCandidateJob) && job.Method.Name == "Run" &&
                        IsMatch((string)job.Args[0], new DateTime(2026, 8, 31), DegreeStatus.FinalYear)
                    ),
                    It.IsAny<EnqueuedState>()));
        }

        [Fact]
        public void AddMember_ValidRequestWithCurrentYear2025AndGraduationYear2025_VerifyHasADegreeWithCorrectGraduationDate()
        {
            // arrange
            _request.GraduationYear = 2025;

            // act
            IActionResult response = _controller.AddMember(_request);

            // assert
            response.Should().BeOfType<NoContentResult>();

            _mockJobClient.Verify(backgroundJobClient =>
                backgroundJobClient.Create(
                    It.Is<Job>(
                        job => job.Type == typeof(UpsertCandidateJob) && job.Method.Name == "Run" &&
                        IsMatch((string)job.Args[0], new DateTime(2025, 8, 31), DegreeStatus.HasDegree)
                    ),
                    It.IsAny<EnqueuedState>()));
        }

        [Fact]
        public void AddMember_ValidRequestWithCurrentYear2025AndGraduationYear2000_VerifyHasADegreeWithCorrectGraduationDate()
        {
            // arrange
            _request.GraduationYear = 2000;

            // act
            IActionResult response = _controller.AddMember(_request);

            // assert
            response.Should().BeOfType<NoContentResult>();

            _mockJobClient.Verify(backgroundJobClient =>
                backgroundJobClient.Create(
                    It.Is<Job>(
                        job => job.Type == typeof(UpsertCandidateJob) && job.Method.Name == "Run" &&
                        IsMatch((string)job.Args[0], new DateTime(2000, 8, 31), DegreeStatus.HasDegree)
                    ),
                    It.IsAny<EnqueuedState>()));
        }

        private static DegreeStatusDomainService SetupDegreeStatusDomainServiceFixture()
        {
            IEnumerable<IChainEvaluationHandler<
                DegreeStatusInferenceRequest, DegreeStatus>> degreeStatusInferenceHandlers
                    = ChainEvaluationHandlerStub.ChainEvaluationHandlersStub<DegreeStatusInferenceRequest, DegreeStatus>();

            return new DegreeStatusDomainService(degreeStatusInferenceHandlers);
        }

        private static bool IsMatch(string candidateJson, DateTime? graduationYear, DegreeStatus degreeStatus)
        {
            var candidate = candidateJson.DeserializeChangeTracked<Candidate>();

            return
                candidate.Qualifications[0].GraduationYear == graduationYear &&
                candidate.Qualifications[0].DegreeStatusId == (int)degreeStatus;
        }
    }
}
