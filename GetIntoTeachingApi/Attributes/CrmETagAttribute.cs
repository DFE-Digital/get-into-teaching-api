using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GetIntoTeachingApi.Attributes
{
    public class CrmETagAttribute : Attribute, IActionFilter
    {
        private readonly JobStorage _hangfireJobStorage;
        private readonly IMetricService _metrics;
        private readonly IEnv _env;

        public CrmETagAttribute()
        {
            _metrics = new MetricService();
            _hangfireJobStorage = JobStorage.Current;
            _env = new Env();
        }

        public CrmETagAttribute(JobStorage hangfireJobStorage, IMetricService metrics, IEnv env)
        {
            _hangfireJobStorage = hangfireJobStorage;
            _metrics = metrics;
            _env = env;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var isGetRequest = context.HttpContext.Request.Method == "GET";
            var canCacheRequest = isGetRequest && !_env.IsDevelopment && !_env.IsTest;

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
                _metrics.CacheLookups.WithLabels("hit").Inc();
                context.Result = new StatusCodeResult((int)HttpStatusCode.NotModified);
            }
            else
            {
                _metrics.CacheLookups.WithLabels("miss").Inc();
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

            return recurringJob?["LastExecution"];
        }
    }
}
