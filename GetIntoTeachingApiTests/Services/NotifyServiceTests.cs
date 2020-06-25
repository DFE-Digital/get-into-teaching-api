using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApiTests.Helpers;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using GetIntoTeachingApi.Utils;
using Xunit;

namespace GetIntoTeachingApiTests.Services
{
    public class NotifyServiceTests
    {
        private readonly Mock<INotificationClientAdapter> _mockNotificationClient;
        private readonly Mock<ILogger<NotifyService>> _mockLogger;
        private readonly NotifyService _service;
        private readonly Dictionary<string, dynamic> _personalisation;

        public NotifyServiceTests()
        {
            var mockEnv = new Mock<IEnv>();
            mockEnv.Setup(m => m.NotifyApiKey).Returns("api_key");
            _mockNotificationClient = new Mock<INotificationClientAdapter>();
            _mockLogger = new Mock<ILogger<NotifyService>>();
            _service = new NotifyService(_mockLogger.Object, _mockNotificationClient.Object, mockEnv.Object);
            _personalisation = new Dictionary<string, dynamic> { { "pin_code", "123456" } };
        }

        [Theory]
        [InlineData(NotifyService.MailingListAddMemberFailedEmailTemplateId, "MailingListAddMemberFailedEmail")]
        [InlineData(NotifyService.NewPinCodeEmailTemplateId, "NewPinCodeEmail")]
        [InlineData(NotifyService.CandidateRegistrationFailedEmailTemplateId, "CandidateRegistrationFailedEmail")]
        [InlineData(NotifyService.TeachingEventRegistrationFailedEmailTemplateId, "TeachingEventRegistrationFailedEmail")]
        [InlineData("UnknownTemplateId", "UnknownTemplate")]
        public void SendEmail_SendsAnEmail(string templateId, string templateDescription)
        {
            _service.SendEmailAsync("email@address.com", templateId, _personalisation);

            _mockNotificationClient.Verify(
                mock => mock.SendEmailAsync(
                    "api_key",
                    "email@address.com",
                    templateId,
                    _personalisation
                )
            );

            _mockLogger.VerifyInformationWasCalled($"NotifyService - Sending Email ({templateDescription})");
        }

        [Fact]
        public void SendEmail_WhenSendingFails_LogsException()
        {
            _mockNotificationClient.Setup(
                mock => mock.SendEmailAsync(
                    "api_key",
                    "email@address.com",
                    NotifyService.NewPinCodeEmailTemplateId,
                    _personalisation
                )
            ).ThrowsAsync(new Exception("bang"));

            _service.SendEmailAsync("email@address.com", NotifyService.NewPinCodeEmailTemplateId, _personalisation).Wait();

            _mockLogger.VerifyWarningWasCalled("NotifyService - Failed to send email");
        }
    }
}
