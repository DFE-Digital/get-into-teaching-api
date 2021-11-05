using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Flurl.Http.Testing;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.FindApply;
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
    public class FindApplyBackfillJobTests
    {
        private readonly Mock<ILogger<FindApplyBackfillJob>> _mockLogger;
        private readonly Mock<IBackgroundJobClient> _mockJobClient;
        private readonly Mock<IEnv> _mockEnv;
        private readonly Mock<IAppSettings> _mockAppSettings;
        private readonly FindApplyBackfillJob _job;

        public FindApplyBackfillJobTests()
        {
            _mockLogger = new Mock<ILogger<FindApplyBackfillJob>>();
            _mockJobClient = new Mock<IBackgroundJobClient>();
            _mockEnv = new Mock<IEnv>();
            _mockAppSettings = new Mock<IAppSettings>();

            _mockEnv.Setup(m => m.FindApplyApiUrl).Returns("https://test.apply.api/candidates-api");
            _mockEnv.Setup(m => m.FindApplyApiKey).Returns("1234567");

            _job = new FindApplyBackfillJob(
                _mockEnv.Object,
                _mockJobClient.Object,
                _mockLogger.Object,
                _mockAppSettings.Object);
        }

        [Fact]
        public void DisableConcurrentExecutionAttribute()
        {
            var type = typeof(FindApplyBackfillJob);

            type.GetMethod("RunAsync").Should().BeDecoratedWith<DisableConcurrentExecutionAttribute>();
        }

        [Fact]
        public async void RunAsync_WhenMultiplePagesAvailable_QueuesCandidateJobsForEachPage()
        {
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
                MockResponse(httpTest, new Response<IEnumerable<Candidate>>() { Data = candidates1 }, 1, 2);
                MockResponse(httpTest, new Response<IEnumerable<Candidate>>() { Data = candidates2 }, 2, 2);
                await _job.RunAsync();
            }

            _mockJobClient.Verify(x => x.Create(
               It.Is<Job>(job => job.Type == typeof(FindApplyCandidateSyncJob) && job.Method.Name == "Run"),
               It.IsAny<ScheduledState>()), Times.Exactly(candidates1.Length + candidates2.Length));

            _mockAppSettings.VerifySet(m => m.IsFindApplyBackfillInProgress = true, Times.Once);
            _mockLogger.VerifyInformationWasCalled("FindApplyBackfillJob - Started");
            _mockLogger.VerifyInformationWasCalled("FindApplyBackfillJob - Syncing 1 Candidates");
            _mockLogger.VerifyInformationWasCalled("FindApplyBackfillJob - Syncing 1 Candidates");
            _mockLogger.VerifyInformationWasCalled("FindApplyBackfillJob - Succeeded");
            _mockAppSettings.VerifySet(m => m.IsFindApplyBackfillInProgress = false, Times.Once);
        }

        [Fact]
        public async void RunAsync_WithNoCandidates_DoesNotQueueJobs()
        {
            using (var httpTest = new HttpTest())
            {
                var response = new Response<IEnumerable<Candidate>>() { Data = Array.Empty<Candidate>() };
                MockResponse(httpTest, response);
                await _job.RunAsync();
            }

            _mockJobClient.Verify(x => x.Create(
               It.Is<Job>(job => job.Type == typeof(FindApplyCandidateSyncJob)),
               It.IsAny<ScheduledState>()), Times.Never);

            _mockAppSettings.VerifySet(m => m.IsFindApplyBackfillInProgress = true, Times.Once);
            _mockLogger.VerifyInformationWasCalled("FindApplyBackfillJob - Started");
            _mockLogger.VerifyInformationWasCalled("FindApplyBackfillJob - Syncing 0 Candidates");
            _mockLogger.VerifyInformationWasCalled("FindApplyBackfillJob - Succeeded");
            _mockAppSettings.VerifySet(m => m.IsFindApplyBackfillInProgress = false, Times.Once);
        }

        private void MockResponse(HttpTest httpTest, Response<IEnumerable<Candidate>> response, int page = 1, int totalPages = 1)
        {
            var json = JsonConvert.SerializeObject(response);
            var headers = new Dictionary<string, int>() { { "Total-Pages", totalPages }, { "Current-Page", page } };

            httpTest
                    .ForCallsTo($"{_mockEnv.Object.FindApplyApiUrl}/candidates")
                    .WithVerb("GET")
                    .WithQueryParam("page", page)
                    .WithQueryParam("updated_since", DateTime.MinValue)
                    .WithHeader("Authorization", $"Bearer {_mockEnv.Object.FindApplyApiKey}")
                    .RespondWith(json, 200, headers);
        }
    }
}
