using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using FluentAssertions;
using Flurl.Http.Testing;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models.Apply;
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
    public class ApplySyncJobTests
    {
        private readonly Mock<GetIntoTeachingApi.Models.IAppSettings> _mockAppSettings;
        private readonly MetricService _metrics;
        private readonly Mock<ILogger<ApplySyncJob>> _mockLogger;
        private readonly Mock<IDateTimeProvider> _mockDateTime;
        private readonly Mock<IBackgroundJobClient> _mockJobClient;
        private readonly Mock<IEnv> _mockEnv;
        private readonly ApplySyncJob _job;

        public ApplySyncJobTests()
        {
            _mockLogger = new Mock<ILogger<ApplySyncJob>>();
            _metrics = new MetricService();
            _mockAppSettings = new Mock<GetIntoTeachingApi.Models.IAppSettings>();
            _mockJobClient = new Mock<IBackgroundJobClient>();
            _mockDateTime = new Mock<IDateTimeProvider>();
            _mockEnv = new Mock<IEnv>();

            _mockEnv.Setup(m => m.ApplyCandidateApiUrl).Returns("https://test.apply.api/candidates-api");
            _mockEnv.Setup(m => m.ApplyCandidateApiKey).Returns("1234567");

            _job = new ApplySyncJob(
                _mockEnv.Object,
                new Mock<IRedisService>().Object,
                _mockLogger.Object,
                _mockJobClient.Object,
                _mockAppSettings.Object,
                _metrics,
                _mockDateTime.Object);
        }

        [Fact]
        public void DisableConcurrentExecutionAttribute()
        {
            var type = typeof(ApplySyncJob);

            type.GetMethod("RunAsync").Should().BeDecoratedWith<DisableConcurrentExecutionAttribute>();
        }

        [Fact]
        public async Task RunAsync_WithUpdatedCandidates_QueuesCandidateJobs()
        {
            var metricCount = _metrics.ApplySyncDuration.Count;
            var lastSyncAt = new DateTime(2020, 1, 1);
            _mockAppSettings.Setup(m => m.ApplyLastSyncAt).Returns(lastSyncAt);
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
               It.Is<Job>(job => job.Type == typeof(ApplyCandidateSyncJob) && job.Method.Name == "Run"),
               It.IsAny<EnqueuedState>()), Times.Exactly(candidates.Length));

            _mockLogger.VerifyInformationWasCalled("ApplySyncJob - Started");
            _mockLogger.VerifyInformationWasCalled("ApplySyncJob - Syncing 2 Candidates");
            _mockLogger.VerifyInformationWasCalled("ApplySyncJob - Succeeded");
            _metrics.ApplySyncDuration.Count.Should().Be(metricCount + 1);
        }

        [Fact]
        public async Task RunAsync_WhenMultiplePagesAvailable_QueuesCandidateJobsForEachPage()
        {
            var lastSyncAt = new DateTime(2020, 1, 1);
            _mockAppSettings.Setup(m => m.ApplyLastSyncAt).Returns(lastSyncAt);
            var candidates1 = new Candidate[]
            {
                new Candidate() { Id = "11111", Attributes = new CandidateAttributes() { Email = "email1@address.com" } },
            };
            var candidates2 = new Candidate[]
            {
                new Candidate() { Id = "11111", Attributes = new CandidateAttributes() { Email = "email1@address.com" } },
            };

            using (var httpTest = new HttpTest())
            {
                MockResponse(httpTest, lastSyncAt, new Response<IEnumerable<Candidate>>() { Data = candidates1 }, 1, 2);
                MockResponse(httpTest, lastSyncAt, new Response<IEnumerable<Candidate>>() { Data = candidates2 }, 2, 2);
                await _job.RunAsync();
            }

            _mockJobClient.Verify(x => x.Create(
               It.Is<Job>(job => job.Type == typeof(ApplyCandidateSyncJob) && job.Method.Name == "Run"),
               It.IsAny<EnqueuedState>()), Times.Exactly(candidates1.Length + candidates2.Length));
        }

        [Fact]
        public async Task RunAsync_WithNoUpdatedCandidates_DoesNotQueueJobs()
        {
            var metricCount = _metrics.ApplySyncDuration.Count;
            var lastSyncAt = new DateTime(2020, 1, 1);
            _mockAppSettings.Setup(m => m.ApplyLastSyncAt).Returns(lastSyncAt);

            using (var httpTest = new HttpTest())
            {
                var response = new Response<IEnumerable<Candidate>>() { Data = Array.Empty<Candidate>() };
                MockResponse(httpTest, lastSyncAt, response);
                await _job.RunAsync();
            }

            _mockJobClient.Verify(x => x.Create(
               It.Is<Job>(job => job.Type == typeof(ApplyCandidateSyncJob)),
               It.IsAny<EnqueuedState>()), Times.Never);

            _mockLogger.VerifyInformationWasCalled("ApplySyncJob - Started");
            _mockLogger.VerifyInformationWasCalled("ApplySyncJob - Syncing 0 Candidates");
            _mockLogger.VerifyInformationWasCalled("ApplySyncJob - Succeeded");
            _metrics.ApplySyncDuration.Count.Should().Be(metricCount + 1);
        }

        [Fact]
        public async Task RunAsync_OnFirstRun_GetsUpdatedSinceNow()
        {
            var now = DateTime.UtcNow;
            _mockDateTime.Setup(m => m.UtcNow).Returns(now);
            _mockAppSettings.Setup(m => m.ApplyLastSyncAt).Returns<DateTime>(null);

            using (var httpTest = new HttpTest())
            {
                var response = new Response<IEnumerable<Candidate>>() { Data = Array.Empty<Candidate>() };
                MockResponse(httpTest, now, response);
                await _job.RunAsync();
            }

            _mockLogger.VerifyInformationWasCalled("ApplySyncJob - Started");
            _mockLogger.VerifyInformationWasCalled("ApplySyncJob - Syncing 0 Candidates");
            _mockLogger.VerifyInformationWasCalled("ApplySyncJob - Succeeded");
        }

        [Fact]
        public async Task RunAsync_OnSuccess_UpdatesLastSyncAt()
        {
            var lastSyncAt = DateTime.UtcNow.AddMonths(-1);
            _mockAppSettings.Setup(m => m.ApplyLastSyncAt).Returns(lastSyncAt);
            var now = DateTime.UtcNow;
            _mockDateTime.Setup(m => m.UtcNow).Returns(now);

            using (var httpTest = new HttpTest())
            {
                var response = new Response<IEnumerable<Candidate>>() { Data = Array.Empty<Candidate>() };
                MockResponse(httpTest, lastSyncAt, response);
                await _job.RunAsync();
            }

            _mockAppSettings.VerifySet(m => m.ApplyLastSyncAt = now, Times.Once);
        }

        private void MockResponse(HttpTest httpTest, DateTime updatedSince, Response<IEnumerable<Candidate>> response, int page = 1, int totalPages = 1)
        {
            var json = JsonConvert.SerializeObject(response);
            var headers = new Dictionary<string, int>() {  { "Total-Pages", totalPages }, { "Current-Page", page } };

            httpTest
                    .ForCallsTo($"{_mockEnv.Object.ApplyCandidateApiUrl}/candidates")
                    .WithVerb("GET")
                    .WithQueryParam("page", page)
                    .WithQueryParam("updated_since", updatedSince)
                    .WithHeader("Authorization", $"Bearer {_mockEnv.Object.ApplyCandidateApiKey}")
                    .RespondWith(json, 200, headers);
        }
    }
}