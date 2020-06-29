using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using GetIntoTeachingApi.Jobs;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GetIntoTeachingApi.Filters
{
    public class CrmETagAttribute : Attribute, IActionFilter
    {
        private readonly JobStorage _hangfireJobStorage;

        public CrmETagAttribute()
        {
            _hangfireJobStorage = JobStorage.Current;
        }

        public CrmETagAttribute(JobStorage hangfireJobStorage)
        {
            _hangfireJobStorage = hangfireJobStorage;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var canCacheRequest = context.HttpContext.Request.Method == "GET";

            if (!canCacheRequest)
            {
                return;
            }

            var path = context.HttpContext.Request.Path.ToString();
            var queryString = context.HttpContext.Request.QueryString.ToString();
            var eTag = GenerateETag($"{path}{queryString}", CrmSyncNextExecutionAt());
            var ifNoneMatchHeader = context.HttpContext.Request.Headers["If-None-Match"].ToString();

            if (ifNoneMatchHeader == eTag)
            {
                context.Result = new StatusCodeResult((int)HttpStatusCode.NotModified);
            }

            context.HttpContext.Response.Headers.Add("ETag", new[] { eTag });
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            var canCacheStatusCode = context.HttpContext.Response.StatusCode == (int)HttpStatusCode.OK;

            if (canCacheStatusCode)
            {
                return;
            }

            // Ensure we never cache a bad response.
            context.HttpContext.Response.Headers.Remove("ETag");
        }

        private static string GenerateETag(string url, string nextSyncAt)
        {
            var text = $"{url}{nextSyncAt}";
            using var sha256 = new SHA256Managed();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));

            return BitConverter.ToString(hash).Replace("-", string.Empty);
        }

        private string CrmSyncNextExecutionAt()
        {
            var connection = _hangfireJobStorage.GetConnection();
            var recurringJob = connection.GetAllEntriesFromHash(
                $"recurring-job:{JobConfiguration.CrmSyncJobId}");

            return recurringJob?["NextExecution"];
        }
    }
}
