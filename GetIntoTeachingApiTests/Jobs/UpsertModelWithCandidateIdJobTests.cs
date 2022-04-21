using System;
using System.Collections.Generic;
using FluentAssertions;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using GetIntoTeachingApiTests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Jobs
{
    public class UpsertModelWithCandidateIdJobTests
    {
        private readonly Mock<IPerformContextAdapter> _mockContext;
        private readonly Mock<IAppSettings> _mockAppSettings;
        private readonly Mock<ICrmService> _mockCrm;
        private readonly Mock<INotifyService> _mockNotifyService;
        private readonly CandidatePrivacyPolicy _policy;
        private readonly IMetricService _metrics;
        private readonly UpsertModelWithCandidateIdJob<CandidatePrivacyPolicy> _job;
        private readonly Mock<ILogger<UpsertModelWithCandidateIdJob<CandidatePrivacyPolicy>>> _mockLogger;

        public UpsertModelWithCandidateIdJobTests()
        {
            _mockContext = new Mock<IPerformContextAdapter>();
            _mockLogger = new Mock<ILogger<UpsertModelWithCandidateIdJob<CandidatePrivacyPolicy>>>();
            _mockAppSettings = new Mock<IAppSettings>();
            _mockCrm = new Mock<ICrmService>();
            _mockNotifyService = new Mock<INotifyService>();
            _metrics = new MetricService();
            _policy = new CandidatePrivacyPolicy() { Id = Guid.NewGuid(), AcceptedAt = DateTime.UtcNow, CandidateId = Guid.NewGuid() };
            _job = new UpsertModelWithCandidateIdJob<CandidatePrivacyPolicy>(
                new Env(), new Mock<IRedisService>().Object, _mockContext.Object, _mockCrm.Object,
                _metrics, _mockLogger.Object, _mockAppSettings.Object, _mockNotifyService.Object);

            _metrics.HangfireJobQueueDuration.RemoveLabelled(new[] { "UpsertModelWithCandidateIdJob<CandidatePrivacyPolicy>" });
            _mockContext.Setup(m => m.GetJobCreatedAt(null)).Returns(DateTime.UtcNow.AddDays(-1));

            _mockAppSettings.Setup(m => m.IsCrmIntegrationPaused).Returns(false);
        }

        [Fact]
        public void Run_OnSuccess_UpsertsModel()
        {
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(0);

            var json = _policy.SerializeChangeTracked();
            _job.Run(json, null);

            _mockCrm.Verify(mock => mock.Save(It.Is<CandidatePrivacyPolicy>(p => IsMatch(_policy, p))), Times.Once);
            _mockLogger.VerifyInformationWasCalled("UpsertModelWithCandidateIdJob<CandidatePrivacyPolicy> - Started (1/24)");
            _mockLogger.VerifyInformationWasCalled($"UpsertModelWithCandidateIdJob<CandidatePrivacyPolicy> - Payload {Redactor.RedactJson(json)}");
            _mockLogger.VerifyInformationWasCalled($"UpsertModelWithCandidateIdJob<CandidatePrivacyPolicy> - Succeeded - {_policy.Id}");
            _metrics.HangfireJobQueueDuration.WithLabels(new[] { "UpsertModelWithCandidateIdJob<CandidatePrivacyPolicy>" }).Count.Should().Be(1);
        }

        [Fact]
        public void Run_OnFailure_EmailsCandidate()
        {
            var candidate = new Candidate() { Id = _policy.CandidateId, Email = "email@address.com" };
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(23);
            _mockCrm.Setup(m => m.GetCandidate((Guid)candidate.Id)).Returns(candidate);

            _job.Run(_policy.SerializeChangeTracked(), null);

            _mockCrm.Verify(mock => mock.Save(It.IsAny<CandidatePrivacyPolicy>()), Times.Never);

            _mockNotifyService.Verify(mock => mock.SendEmailAsync(candidate.Email,
                NotifyService.SignUpPartiallyFailedTemplateId, It.IsAny<Dictionary<string, dynamic>>()));
            _mockLogger.VerifyInformationWasCalled("UpsertModelWithCandidateIdJob<CandidatePrivacyPolicy> - Started (24/24)");
            _mockLogger.VerifyInformationWasCalled("UpsertModelWithCandidateIdJob<CandidatePrivacyPolicy> - Deleted");
            _metrics.HangfireJobQueueDuration.WithLabels(new[] { "UpsertModelWithCandidateIdJob<CandidatePrivacyPolicy>" }).Count.Should().Be(1);
        }

        [Fact]
        public void Run_WhenCrmIntegrationPaused_Aborts()
        {
            _mockAppSettings.Setup(m => m.IsCrmIntegrationPaused).Returns(true);

            var json = _policy.SerializeChangeTracked();
            Action action = () => _job.Run(json, null);

            action.Should().Throw<InvalidOperationException>()
                .WithMessage("UpsertModelWithCandidateIdJob<CandidatePrivacyPolicy> - Aborting (CRM integration paused).");
        }

        private static bool IsMatch(object objectA, object objectB)
        {
            objectA.Should().BeEquivalentTo(objectB);
            return true;
        }
    }
}
