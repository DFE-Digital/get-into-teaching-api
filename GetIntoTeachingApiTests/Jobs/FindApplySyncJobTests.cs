﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Flurl.Http.Testing;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models.FindApply;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using GetIntoTeachingApiTests.Helpers;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace GetIntoTeachingApiTests.Jobs
{
    public class FindApplySyncJobTests
    {
        private readonly Mock<GetIntoTeachingApi.Models.IAppSettings> _mockAppSettings;
        private readonly MetricService _metrics;
        private readonly Mock<ILogger<FindApplySyncJob>> _mockLogger;
        private readonly Mock<IDateTimeProvider> _mockDateTime;
        private readonly Mock<IBackgroundJobClient> _mockJobClient;
        private readonly Mock<IEnv> _mockEnv;
        private readonly FindApplySyncJob _job;

        public FindApplySyncJobTests()
        {
            _mockLogger = new Mock<ILogger<FindApplySyncJob>>();
            _metrics = new MetricService();
            _mockAppSettings = new Mock<GetIntoTeachingApi.Models.IAppSettings>();
            _mockJobClient = new Mock<IBackgroundJobClient>();
            _mockDateTime = new Mock<IDateTimeProvider>();
            _mockEnv = new Mock<IEnv>();

            _mockEnv.Setup(m => m.FindApplyApiUrl).Returns("https://test.apply.api/candidates-api");
            _mockEnv.Setup(m => m.FindApplyApiKey).Returns("1234567");

            _job = new FindApplySyncJob(
                _mockEnv.Object,
                _mockLogger.Object,
                _mockJobClient.Object,
                _mockAppSettings.Object,
                _metrics,
                _mockDateTime.Object);
        }

        [Fact]
        public void DisableConcurrentExecutionAttribute()
        {
            var type = typeof(FindApplySyncJob);

            type.GetMethod("RunAsync").Should().BeDecoratedWith<DisableConcurrentExecutionAttribute>();
        }

        [Fact]
        public async void RunAsync_WithUpdatedCandidates_QueuesCandidateJobs()
        {
            var metricCount = _metrics.FindApplySyncDuration.Count;
            var lastSyncAt = new DateTime(2020, 1, 1);
            _mockAppSettings.Setup(m => m.FindApplyLastSyncAt).Returns(lastSyncAt);
            var candidates = new Candidate[]
            {
                new Candidate() { Id = "11111", Attributes = new CandidateAttributes() { Email = "email1@address.com" } },
                new Candidate() { Id = "22222", Attributes = new CandidateAttributes() { Email = "email2@address.com" } },
            };

            using (var httpTest = new HttpTest())
            {
                var response = new Response<IEnumerable<Candidate>>() { Data = candidates };
                MockResponse(httpTest, lastSyncAt, response);
                await _job.RunAsync();
            }

            _mockJobClient.Verify(x => x.Create(
               It.Is<Job>(job => job.Type == typeof(FindApplyCandidateSyncJob) && job.Method.Name == "Run"),
               It.IsAny<EnqueuedState>()), Times.Exactly(candidates.Count()));

            _mockLogger.VerifyInformationWasCalled("FindApplySyncJob - Started");
            _mockLogger.VerifyInformationWasCalled("FindApplySyncJob - Syncing 2 Candidates");
            _mockLogger.VerifyInformationWasCalled("FindApplySyncJob - Succeeded");
            _metrics.FindApplySyncDuration.Count.Should().Be(metricCount + 1);
        }

        [Fact]
        public async void RunAsync_WithNoUpdatedCandidates_DoesNotQueueJobs()
        {
            var metricCount = _metrics.FindApplySyncDuration.Count;
            var lastSyncAt = new DateTime(2020, 1, 1);
            _mockAppSettings.Setup(m => m.FindApplyLastSyncAt).Returns(lastSyncAt);

            using (var httpTest = new HttpTest())
            {
                var response = new Response<IEnumerable<Candidate>>() { Data = new Candidate[0] };
                MockResponse(httpTest, lastSyncAt, response);
                await _job.RunAsync();
            }

            _mockJobClient.Verify(x => x.Create(
               It.Is<Job>(job => job.Type == typeof(FindApplyCandidateSyncJob)),
               It.IsAny<EnqueuedState>()), Times.Never);

            _mockLogger.VerifyInformationWasCalled("FindApplySyncJob - Started");
            _mockLogger.VerifyInformationWasCalled("FindApplySyncJob - Syncing 0 Candidates");
            _mockLogger.VerifyInformationWasCalled("FindApplySyncJob - Succeeded");
            _metrics.FindApplySyncDuration.Count.Should().Be(metricCount + 1);
        }

        [Fact]
        public async void RunAsync_OnFirstRun_GetsUpdatedSinceNow()
        {
            var now = DateTime.UtcNow;
            _mockDateTime.Setup(m => m.UtcNow).Returns(now);
            _mockAppSettings.Setup(m => m.FindApplyLastSyncAt).Returns<DateTime>(null);

            using (var httpTest = new HttpTest())
            {
                var response = new Response<IEnumerable<Candidate>>() { Data = new Candidate[0] };
                MockResponse(httpTest, now, response);
                await _job.RunAsync();
            }

            _mockLogger.VerifyInformationWasCalled("FindApplySyncJob - Started");
            _mockLogger.VerifyInformationWasCalled("FindApplySyncJob - Syncing 0 Candidates");
            _mockLogger.VerifyInformationWasCalled("FindApplySyncJob - Succeeded");
        }

        [Fact]
        public async void RunAsync_OnSuccess_UpdatesLastSyncAt()
        {
            var lastSyncAt = DateTime.UtcNow.AddMonths(-1);
            _mockAppSettings.Setup(m => m.FindApplyLastSyncAt).Returns(lastSyncAt);
            var now = DateTime.UtcNow;
            _mockDateTime.Setup(m => m.UtcNow).Returns(now);

            using (var httpTest = new HttpTest())
            {
                var response = new Response<IEnumerable<Candidate>>() { Data = new Candidate[0] };
                MockResponse(httpTest, lastSyncAt, response);
                await _job.RunAsync();
            }

            _mockAppSettings.VerifySet(m => m.FindApplyLastSyncAt = now, Times.Once);
        }

        private void MockResponse(HttpTest httpTest, DateTime updatedSince, Response<IEnumerable<Candidate>> response)
        {
            var json = JsonConvert.SerializeObject(response);

            httpTest
                    .ForCallsTo($"{_mockEnv.Object.FindApplyApiUrl}/candidates")
                    .WithVerb("GET")
                    .WithQueryParam("updated_since", updatedSince)
                    .WithHeader("Authorization", $"Bearer {_mockEnv.Object.FindApplyApiKey}")
                    .RespondWith(json, 200);
        }
    }
}