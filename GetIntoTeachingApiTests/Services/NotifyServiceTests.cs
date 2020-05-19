using GetIntoTeachingApi.Adapters;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApiTests.Utils;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace GetIntoTeachingApiTests.Services
{
    public class NotifyServiceTests : IDisposable
    {
        private readonly string _previousNotifyApiKey;
        private readonly Mock<INotificationClientAdapter> _mockNotificationClient;
        private readonly Mock<ILogger<NotifyService>> _mockLogger;
        private readonly NotifyService _service;
        private readonly Dictionary<string, dynamic> _personalisation;

        public NotifyServiceTests()
        {
            _previousNotifyApiKey = Environment.GetEnvironmentVariable("NOTIFY_API_KEY");
            Environment.SetEnvironmentVariable("NOTIFY_API_KEY", "api_key");

            _mockNotificationClient = new Mock<INotificationClientAdapter>();
            _mockLogger = new Mock<ILogger<NotifyService>>();
            _service = new NotifyService(_mockLogger.Object, _mockNotificationClient.Object);
            _personalisation = new Dictionary<string, dynamic> { { "pin_code", "123456" } };
        }

        public void Dispose()
        {
            Environment.SetEnvironmentVariable("NOTIFY_API_KEY", _previousNotifyApiKey);
        }

        [Fact]
        public void SendEmail_SendsAnEmail()
        {
            _service.SendEmail("email@address.com", NotifyService.NewPinCodeEmailTemplateId, _personalisation);

            _mockNotificationClient.Verify(
                mock => mock.SendEmailAsync(
                    "api_key",
                    "email@address.com", 
                    NotifyService.NewPinCodeEmailTemplateId, 
                    _personalisation
                )
            );
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

            _service.SendEmail("email@address.com", NotifyService.NewPinCodeEmailTemplateId, _personalisation);

            _mockLogger.VerifyWarningWasCalled("NotifyService - Failed to send email");
        }
    }
}
