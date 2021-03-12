using System.IO;
using System.Threading.Tasks;
using GetIntoTeachingApi.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IO;

namespace GetIntoTeachingApi.Middleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

        public RequestResponseLoggingMiddleware(
            RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        }

        public async Task Invoke(HttpContext context)
        {
            await LogRequest(context);
            await LogResponse(context);
        }

        private static string ReadStream(Stream stream)
        {
            const int bufferSize = 4096;

            stream.Seek(0, SeekOrigin.Begin);

            using var writer = new StringWriter();
            using var reader = new StreamReader(stream);
            var chunk = new char[bufferSize];
            int chunkSize;

            do
            {
                chunkSize = reader.ReadBlock(chunk, 0, bufferSize);
                writer.Write(chunk, 0, chunkSize);
            }
            while (chunkSize > 0);

            return writer.ToString();
        }

        private void LogInformation(string identifier, string payload, HttpContext context)
        {
            var info = new
            {
                context.Request.Scheme,
                context.Request.Host,
                context.Request.Path,
                context.Request.QueryString,
                Payload = Redactor.RedactJson(payload),
            };

            _logger.LogInformation($"{identifier}: {info}");
        }

        private async Task LogRequest(HttpContext context)
        {
            context.Request.EnableBuffering();

            // Copy request body stream, resetting position for next middleware.
            await using var stream = _recyclableMemoryStreamManager.GetStream();
            await context.Request.Body.CopyToAsync(stream);
            context.Request.Body.Position = 0;

            LogInformation("HTTP Request", ReadStream(stream), context);
        }

        private async Task LogResponse(HttpContext context)
        {
            // Keep track of the original response body stream (its read-once).
            var bodyStream = context.Response.Body;

            // Get a new stream.
            await using var stream = _recyclableMemoryStreamManager.GetStream();

            // Write the downstream response into our stream.
            context.Response.Body = stream;
            await _next(context);

            // Extract response body as text, resetting position for next middleware.
            stream.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(stream).ReadToEndAsync();
            stream.Seek(0, SeekOrigin.Begin);

            LogInformation("HTTP Response", text, context);

            // Copy back into the original response body stream.
            await stream.CopyToAsync(bodyStream);
        }
    }
}
