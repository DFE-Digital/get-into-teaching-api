﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Flurl.Http.Testing;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models;
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
    public class ApplyBackfillJobTests
    {
        //    private readonly Mock<ILogger<ApplyBackfillJob>> _mockLogger;
        //    private readonly Mock<IBackgroundJobClient> _mockJobClient;
        //    private readonly Mock<IEnv> _mockEnv;
        //    private readonly Mock<IAppSettings> _mockAppSettings;
        //    private readonly ApplyBackfillJob _job;

        //    public ApplyBackfillJobTests()
        //    {
        //        _mockLogger = new Mock<ILogger<ApplyBackfillJob>>();
        //        _mockJobClient = new Mock<IBackgroundJobClient>();
        //        _mockEnv = new Mock<IEnv>();
        //        _mockAppSettings = new Mock<IAppSettings>();

        //        _mockEnv.Setup(m => m.ApplyCandidateApiUrl).Returns("https://test.apply.api/candidates-api");
        //        _mockEnv.Setup(m => m.ApplyCandidateApiKey).Returns("1234567");

        //        _job = new ApplyBackfillJob(
        //            _mockEnv.Object,
        //            new Mock<IRedisService>().Object,
        //            _mockJobClient.Object,
        //            _mockLogger.Object,
        //            _mockAppSettings.Object);
        //    }

        //    [Fact]
        //    public void DisableConcurrentExecutionAttribute()
        //    {
        //        var type = typeof(ApplyBackfillJob);

        //        type.GetMethod("RunAsync").Should().BeDecoratedWith<DisableConcurrentExecutionAttribute>();
        //    }

        //    [Fact]
        //    public async void RunAsync_WhenMultiplePagesAvailable_QueuesCandidateJobsForEachPage()
        //    {
        //        var updatedSince = DateTime.MinValue;
        //        var candidates1 = new Candidate[]
        //        {
        //            new Candidate() { Id = "11111", Attributes = new CandidateAttributes() { Email = "email1@address.com" } },
        //        };
        //        var candidates2 = new Candidate[]
        //        {
        //            new Candidate() { Id = "11111", Attributes = new CandidateAttributes() { Email = "email1@address.com" } },
        //        };

        //        using (var httpTest = new HttpTest())
        //        {
        //            MockResponse(httpTest, new Response<IEnumerable<Candidate>>() { Data = candidates1 }, 1, 2);
        //            MockResponse(httpTest, new Response<IEnumerable<Candidate>>() { Data = candidates2 }, 2, 2);
        //            await _job.RunAsync(updatedSince);
        //        }

        //        _mockJobClient.Verify(x => x.Create(
        //           It.Is<Job>(job => job.Type == typeof(ApplyCandidateSyncJob) && job.Method.Name == "Run"),
        //           It.IsAny<ScheduledState>()), Times.Exactly(candidates1.Length + candidates2.Length));

        //        _mockAppSettings.VerifySet(m => m.IsApplyBackfillInProgress = true, Times.Once);
        //        _mockLogger.VerifyInformationWasCalled("ApplyBackfillJob - Started");
        //        _mockLogger.VerifyInformationWasCalled("ApplyBackfillJob - Syncing 1 Candidates");
        //        _mockLogger.VerifyInformationWasCalled("ApplyBackfillJob - Syncing 1 Candidates");
        //        _mockLogger.VerifyInformationWasCalled("ApplyBackfillJob - Succeeded");
        //        _mockAppSettings.VerifySet(m => m.IsApplyBackfillInProgress = false, Times.Once);
        //    }

        //    [Fact]
        //    public async void RunAsync_WithNoCandidates_DoesNotQueueJobs()
        //    {
        //        var updatedSince = DateTime.MinValue;
        //        using (var httpTest = new HttpTest())
        //        {
        //            var response = new Response<IEnumerable<Candidate>>() { Data = Array.Empty<Candidate>() };
        //            MockResponse(httpTest, response);
        //            await _job.RunAsync(updatedSince);
        //        }

        //        _mockJobClient.Verify(x => x.Create(
        //           It.Is<Job>(job => job.Type == typeof(ApplyCandidateSyncJob)),
        //           It.IsAny<ScheduledState>()), Times.Never);

        //        _mockAppSettings.VerifySet(m => m.IsApplyBackfillInProgress = true, Times.Once);
        //        _mockLogger.VerifyInformationWasCalled("ApplyBackfillJob - Started");
        //        _mockLogger.VerifyInformationWasCalled("ApplyBackfillJob - Syncing 0 Candidates");
        //        _mockLogger.VerifyInformationWasCalled("ApplyBackfillJob - Succeeded");
        //        _mockAppSettings.VerifySet(m => m.IsApplyBackfillInProgress = false, Times.Once);
        //    }

        //    private void MockResponse(HttpTest httpTest, Response<IEnumerable<Candidate>> response, int page = 1, int totalPages = 1)
        //    {
        //        var json = JsonConvert.SerializeObject(response);
        //        var headers = new Dictionary<string, int>() { { "Total-Pages", totalPages }, { "Current-Page", page } };

        //        httpTest
        //                .ForCallsTo($"{_mockEnv.Object.ApplyCandidateApiUrl}/candidates")
        //                .WithVerb("GET")
        //                .WithQueryParam("page", page)
        //                .WithQueryParam("updated_since", DateTime.MinValue)
        //                .WithHeader("Authorization", $"Bearer {_mockEnv.Object.ApplyCandidateApiKey}")
        //                .RespondWith(json, 200, headers);
        //    }
    }
}