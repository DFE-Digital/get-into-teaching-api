using System;
using System.Collections.Generic;
using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Models;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using GetIntoTeachingApiTests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Jobs
{
    public class MailingListAddMemberJobTests
    {
        private readonly Mock<IPerformContextAdapter> _mockContext;
        private readonly Mock<ICrmService> _mockCrm;
        private readonly Mock<INotifyService> _mockNotifyService;
        private readonly Mock<ILogger<MailingListAddMemberJob>> _mockLogger;
        private readonly MailingListAddMemberRequest _request;
        private readonly MailingListAddMemberJob _job;

        public MailingListAddMemberJobTests()
        {
            _mockContext = new Mock<IPerformContextAdapter>();
            _mockCrm = new Mock<ICrmService>();
            _mockNotifyService = new Mock<INotifyService>();
            _mockLogger = new Mock<ILogger<MailingListAddMemberJob>>();
            _request = new MailingListAddMemberRequest()
            {
                Email = "test@test.com",
                FirstName = "first",
                LastName = "last",
                Telephone = "1234",
                AddressPostcode = "KY11 9YU",
                PreferredTeachingSubjectId = Guid.NewGuid(),
                PrivacyPolicy = new CandidatePrivacyPolicy() { AcceptedPolicyId = Guid.NewGuid() }
            };
            _job = new MailingListAddMemberJob(new Env(), _mockCrm.Object, _mockNotifyService.Object,
                _mockContext.Object, _mockLogger.Object);
        }

        [Fact]
        public void Run_OnSuccessWithExistingCandidate_AddsAsMember()
        {
            var candidateId = Guid.NewGuid();
            var candidate = new Candidate() { Id = candidateId };
            _request.CandidateId = candidateId;
            _mockCrm.Setup(m => m.Save(It.Is<Candidate>(c => VerifyUpdatedCandidate(c, _request.Telephone))));
            _mockCrm.Setup(m => m.GetCandidate(candidateId)).Returns(candidate);
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(0);

            _job.Run(_request, null);

            _mockLogger.VerifyInformationWasCalled("MailingListAddMemberJob - Started (1/24)");
            _mockLogger.VerifyInformationWasCalled("MailingListAddMemberJob - Succeeded");
        }

        [Fact]
        public void Run_OnSuccessWithExistingCandidateAndNewTelephoneIsNull_DoesNotOverwriteTelephone()
        {
            var candidateId = Guid.NewGuid();
            var candidate = new Candidate() { Id = candidateId, Telephone = "1234" };
            _request.CandidateId = candidateId;
            _request.Telephone = null;
            _mockCrm.Setup(m => m.Save(It.Is<Candidate>(c => VerifyUpdatedCandidate(c, c.Telephone))));
            _mockCrm.Setup(m => m.GetCandidate(candidateId)).Returns(candidate);
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(0);

            _job.Run(_request, null);

            _mockLogger.VerifyInformationWasCalled("MailingListAddMemberJob - Started (1/24)");
            _mockLogger.VerifyInformationWasCalled("MailingListAddMemberJob - Succeeded");
        }

        [Fact]
        public void Run_OnSuccessWithNewCandidate_AddsAsMember()
        {
            var candidateId = Guid.NewGuid();
            _mockCrm.Setup(m => m.Save(It.Is<Candidate>(c => VerifyUpdatedCandidate(c, _request.Telephone))))
                .Callback<BaseModel>(c => c.Id = candidateId);
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(0);

            _job.Run(_request, null);

            _mockCrm.Verify(m => m.GetCandidate(It.IsAny<Guid>()), Times.Never);
            _mockLogger.VerifyInformationWasCalled("MailingListAddMemberJob - Started (1/24)");
            _mockLogger.VerifyInformationWasCalled("MailingListAddMemberJob - Succeeded");
        }

        [Fact]
        public void Run_OnFailure_EmailsCandidate()
        {
            _mockContext.Setup(m => m.GetRetryCount(null)).Returns(23);

            _job.Run(_request, null);

            _mockNotifyService.Verify(mock => mock.SendEmailAsync(_request.Email,
                NotifyService.MailingListAddMemberFailedEmailTemplateId, It.IsAny<Dictionary<string, dynamic>>()));
            _mockLogger.VerifyInformationWasCalled("MailingListAddMemberJob - Started (24/24)");
            _mockLogger.VerifyInformationWasCalled("MailingListAddMemberJob - Deleted");
        }

        private bool VerifyUpdatedCandidate(Candidate candidate, string expectedTelephone)
        {
            return candidate.FirstName == _request.FirstName &&
                   candidate.LastName == _request.LastName &&
                   candidate.Email == _request.Email &&
                   candidate.Telephone == expectedTelephone &&
                   candidate.AddressPostcode == _request.AddressPostcode &&
                   candidate.PreferredTeachingSubjectId == _request.PreferredTeachingSubjectId &&
                   candidate.PrivacyPolicy == _request.PrivacyPolicy;
        }
    }
}