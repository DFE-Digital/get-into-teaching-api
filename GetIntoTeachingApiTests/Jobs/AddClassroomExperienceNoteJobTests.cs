using FluentAssertions;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Models.Crm;
using GetIntoTeachingApi.Models.SchoolsExperience;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using GetIntoTeachingApiTests.Helpers;
using Hangfire;
using Hangfire.Common;
using Hangfire.Server;
using Hangfire.States;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

namespace GetIntoTeachingApiTests.Jobs
{
    public class AddClassroomExperienceNoteJobTests
    {
        private readonly Mock<IEnv> _mockEnv;
        private readonly Mock<IPerformContextAdapter> _mockContext;
        private readonly Mock<IAppSettings> _mockAppSettings;
        private readonly Mock<ICrmService> _mockCrm;
        private readonly Mock<IBackgroundJobClient> _mockJobClient;
        private readonly AddClassroomExperienceNoteJob _job;

        public AddClassroomExperienceNoteJobTests()
        {
            _mockEnv = new Mock<IEnv>();
            _mockContext = new Mock<IPerformContextAdapter>();
            _mockAppSettings = new Mock<IAppSettings>();
            _mockCrm = new Mock<ICrmService>();
            _mockJobClient = new Mock<IBackgroundJobClient>();

            _job = new AddClassroomExperienceNoteJob(
                _mockEnv.Object,
                _mockContext.Object,
                _mockAppSettings.Object,
                _mockCrm.Object,
                _mockJobClient.Object);
        }

        [Fact]
        public void Run_WhenCrmIntegrationPaused_ThrowsException()
        {
            _mockAppSettings.Setup(mock => mock.IsCrmIntegrationPaused).Returns(true);

            Action job = () => _job.Run(null, new ClassroomExperienceNote(), Guid.NewGuid());

            job.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("AddClassroomExperienceNoteJob - Aborting (CRM integration paused).");
        }

        [Fact]
        public void Run_WhenCandidateReturnsNullAndLessThanThreeTries_ThrowsInformationalException()
        {
            _mockCrm.Setup(mock => mock.GetCandidate(It.IsAny<Guid>())).Returns((Candidate)null);
            _mockContext.Setup(mock => mock.GetRetryCount(null)).Returns(2);

            Action job = () => _job.Run(null, new ClassroomExperienceNote(), Guid.NewGuid());

            job.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("AddClassroomExperienceNoteJob - Candidate not found (may be in concurrent job queue)");
            _mockJobClient.Verify(x => x.Create(
                It.Is<Job>(job => job.Type == typeof(UpsertCandidateJob) && job.Method.Name == "Run"),
                It.IsAny<EnqueuedState>()), Times.Never);
        }

        [Fact]
        public void Run_WhenCandidateReturnsNullAndMoreThanThreeTries_ThrowsException()
        {
            _mockCrm.Setup(mock => mock.GetCandidate(It.IsAny<Guid>())).Returns((Candidate)null);
            _mockContext.Setup(mock => mock.GetRetryCount(null)).Returns(3);

            Action job = () => _job.Run(null, new ClassroomExperienceNote(), Guid.NewGuid());

            job.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("AddClassroomExperienceNoteJob - Candidate not found");
            _mockJobClient.Verify(x => x.Create(
                It.Is<Job>(job => job.Type == typeof(UpsertCandidateJob) && job.Method.Name == "Run"),
                It.IsAny<EnqueuedState>()), Times.Never);
        }

        [Fact]
        public void Run_OnSuccess_QueuesCandidateUpsertJob()
        {
            var note = new ClassroomExperienceNote() { Action = "REQUESTED", SchoolName = "School Name", SchoolUrn = 123456 };
            var candidate = new Candidate() { Id = Guid.NewGuid() };
            _mockCrm.Setup(mock => mock.GetCandidate(candidate.Id.Value)).Returns(candidate);

            _job.Run(null, note, candidate.Id.Value);

            candidate.AddClassroomExperienceNote(note);
            _mockJobClient.Verify(x => x.Create(
               It.Is<Job>(job => job.Type == typeof(UpsertCandidateJob) &&
                                 job.Method.Name == "Run" &&
                                 IsMatch(candidate, (string)job.Args[0])),
               It.IsAny<EnqueuedState>()), Times.Once);
        }

        private static bool IsMatch(Candidate candidateA, string candidateBJson)
        {
            var candidateB = candidateBJson.DeserializeChangeTracked<Candidate>();
            candidateA.Should().BeEquivalentTo(candidateB);
            return true;
        }
    }
}
