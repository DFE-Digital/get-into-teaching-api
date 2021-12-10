using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GetIntoTeachingApi.Middleware;
using GetIntoTeachingApiTests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Middleware
{
    public class RequestResponseLoggingMiddlewareTests
    {
        private readonly Mock<ILogger<RequestResponseLoggingMiddleware>> _mockLogger;
        private readonly Mock<IRequestResponseLoggingConfiguration> _mockConfig;
        private readonly DefaultHttpContext _context;

        public RequestResponseLoggingMiddlewareTests()
        {
            _mockLogger = new Mock<ILogger<RequestResponseLoggingMiddleware>>();
            _mockConfig = new Mock<IRequestResponseLoggingConfiguration>();
            _mockConfig.Setup(m => m.CompactLoggingPatterns).Returns(Array.Empty<Regex>());
            _context = new DefaultHttpContext();

            _context.Request.Scheme = "https";
            _context.Request.Method = "get";
            _context.Request.Host = new HostString("host", 80);
            _context.Request.Path = "/path";
            _context.Request.QueryString = new QueryString("?key=value");
        }

        [Fact]
        public async void Invoke_WithJsonPayloadAndCompactLoggingPath_LogsRequestAndResponseWithoutPayload()
        {
            var regex = new Regex(@"^GET /path", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            _mockConfig.Setup(m => m.CompactLoggingPatterns).Returns(new Regex[] { regex });

            string json = JsonSerializer.Serialize(new
                {
                    key = "value",
                }
            );

            await MiddlewareWithPayload(json).Invoke(_context);

            var expectedInfo = new
            {
                _context.Request.Scheme,
                _context.Request.Host,
                _context.Request.Path,
                _context.Request.QueryString,
                Payload = string.Empty,
            };

            _mockLogger.VerifyInformationWasCalled($"HTTP Request: {expectedInfo}");
            _mockLogger.VerifyInformationWasCalled($"HTTP Response: {expectedInfo}");
        }

        [Fact]
        public async void Invoke_WithJsonPayload_LogsRequestAndResponseWithRedactedPayloads()
        {
            string json = JsonSerializer.Serialize(new
                {
                    password = "abc123",
                    addressPostcode = "TE7 1NG",
                }
            );

            string redactedJson = JsonSerializer.Serialize(
                new
                {
                    password = "******",
                    addressPostcode = "TE7 1NG",
                }
            );

            await MiddlewareWithPayload(json).Invoke(_context);

            var expectedInfo = new
            {
                _context.Request.Scheme,
                _context.Request.Host,
                _context.Request.Path,
                _context.Request.QueryString,
                Payload = redactedJson,
            };

            _mockLogger.VerifyInformationWasCalled($"HTTP Request: {expectedInfo}");
            _mockLogger.VerifyInformationWasCalled($"HTTP Response: {expectedInfo}");
        }

        [Fact]
        public async void Invoke_WithNonJsonPayload_LogsRequestAndResponseWithEmptyPayloads()
        {
            var text = "my password is 123456";

            await MiddlewareWithPayload(text).Invoke(_context);

            var expectedInfo = new
            {
                _context.Request.Scheme,
                _context.Request.Host,
                _context.Request.Path,
                _context.Request.QueryString,
                Payload = string.Empty,
            };

            _mockLogger.VerifyInformationWasCalled($"HTTP Request: {expectedInfo}");
            _mockLogger.VerifyInformationWasCalled($"HTTP Response: {expectedInfo}");
        }

        private RequestResponseLoggingMiddleware MiddlewareWithPayload(string payload)
        {
            _context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(payload));

            return new RequestResponseLoggingMiddleware(
                next: (innerHttpContext) =>
                {
                    innerHttpContext.Response.WriteAsync(payload);
                    return Task.CompletedTask;
                },
                _mockLogger.Object,
                _mockConfig.Object
            );
        }
    }
}
