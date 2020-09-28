using System.Collections.Generic;
using System.Linq;
using System.Net;
using FluentAssertions;
using GetIntoTeachingApi.Filters;
using GetIntoTeachingApi.Jobs;
using GetIntoTeachingApi.Services;
using GetIntoTeachingApi.Utils;
using Hangfire;
using Hangfire.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Moq;
using Xunit;

namespace GetIntoTeachingApiTests.Filters
{
    public class CrmETagAttributeTests
    {
        private readonly CrmETagAttribute _filter;
        private readonly ActionExecutingContext _actionExecutingContext;
        private readonly ActionExecutedContext _actionExecutedContext;
        private readonly StatusCodeResult _originalResult;
        private readonly Mock<HttpContext> _mockHttpContext;
        private readonly Mock<IEnv> _mockEnv;
        private readonly Mock<IStorageConnection> _mockStorageConnection;

        public CrmETagAttributeTests()
        {
            var actionContext = new ActionContext(
                Mock.Of<HttpContext>(),
                Mock.Of<RouteData>(),
                Mock.Of<ActionDescriptor>(),
                new ModelStateDictionary()
            );

            _actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                Mock.Of<Controller>()
            );

            _actionExecutedContext = new ActionExecutedContext(
                actionContext, 
                new List<IFilterMetadata>(), 
                Mock.Of<Controller>()
            );

            _mockEnv = new Mock<IEnv>();
            _mockEnv.Setup(m => m.IsTest).Returns(false);

            _mockHttpContext = new Mock<HttpContext>();
            _originalResult = new StatusCodeResult((int)HttpStatusCode.OK);
            _actionExecutingContext.HttpContext = _mockHttpContext.Object;
            _actionExecutedContext.HttpContext = _mockHttpContext.Object;
            _actionExecutingContext.Result = _originalResult;

            _mockStorageConnection = new Mock<IStorageConnection>();
            var mockStorage = new Mock<JobStorage>();
            mockStorage.Setup(m => m.GetConnection()).Returns(_mockStorageConnection.Object);

            _filter = new CrmETagAttribute(mockStorage.Object, new MetricService(), _mockEnv.Object);
        }

        [Theory]
        [InlineData("POST", false, false)]
        [InlineData("PUT", false, false)]
        [InlineData("GET", true, false)]
        [InlineData("GET", false, true)]
        public void OnActionExecuting_IgnoresIfNotGetOrIsDevelopmentOrTestEnv(string method, bool isDevelopment, bool isTest)
        {
            _mockEnv.Setup(m => m.IsDevelopment).Returns(isDevelopment);
            _mockEnv.Setup(m => m.IsTest).Returns(isTest);
            _mockHttpContext.Setup(m => m.Request.Method).Returns(method);

            _filter.OnActionExecuting(_actionExecutingContext);

            _actionExecutingContext.Result.Should().Be(_originalResult);
           _mockHttpContext.Verify(m => m.Response.Headers.Add("ETag", It.IsAny<StringValues>()), Times.Never);
        }

        [Fact]
        public void OnActionExecuting_DifferentPaths_RespondsWithUniqueETags()
        {
            var eTags = new List<string>();
            _mockHttpContext.Setup(m => m.Request.Headers["If-None-Match"]).Returns<string>(null);
            _mockHttpContext.Setup(m => m.Request.Method).Returns("GET");
            _mockHttpContext.Setup(m => m.Request.QueryString).Returns(new QueryString());
            _mockHttpContext.SetupSequence(m => m.Request.Path)
                .Returns("/path/to/resource1")
                .Returns("/path/to/resource2");
            _mockHttpContext.Setup(m => m.Response.Headers.Add("ETag", It.IsAny<StringValues>()))
                .Callback<string, StringValues>((key, values) => eTags.AddRange(values.ToArray()));

            _filter.OnActionExecuting(_actionExecutingContext);
            _filter.OnActionExecuting(_actionExecutingContext);

            eTags.Distinct().Count().Should().Be(2);
        }

        [Fact]
        public void OnActionExecuting_BeforeFirstCrmSyncJobHasRan_RespondsWithAnETag()
        {
            const string eTag = "2B75BAF65CCDB4D32112CBEA0D66505CD5EC5A05D699EF8ED79169ED128D2F38";
            _mockHttpContext.Setup(m => m.Request.Headers["If-None-Match"]).Returns<string>(null);
            _mockHttpContext.Setup(m => m.Request.Method).Returns("GET");
            _mockHttpContext.Setup(m => m.Request.QueryString).Returns(new QueryString());
            _mockHttpContext.Setup(m => m.Request.Path).Returns("/path/to/resource1");
            _mockHttpContext.Setup(m => m.Response.Headers.Add("ETag", It.IsAny<StringValues>()));

            _filter.OnActionExecuting(_actionExecutingContext);

            _mockHttpContext.Verify(m => m.Response.Headers.Add("ETag", It.Is<StringValues>(v => v.Contains(eTag))));
        }

        [Fact]
        public void OnActionExecuting_AfterCrmSyncJobHasRan_RespondsWithNewETag()
        {
            const string eTag = "03AC2E21CC0F0298899728A7648684A16B08DF1EA22AAD296AC916B50D6A9FE1";
            var jobDetails = new Dictionary<string, string>() { { "LastExecution", "2020-01-01 19:52:44" } };
            _mockStorageConnection.Setup(m => m.GetAllEntriesFromHash(
                $"recurring-job:{JobConfiguration.CrmSyncJobId}")).Returns(jobDetails);
            _mockHttpContext.Setup(m => m.Request.Headers["If-None-Match"]).Returns<string>(null);
            _mockHttpContext.Setup(m => m.Request.Method).Returns("GET");
            _mockHttpContext.Setup(m => m.Request.QueryString).Returns(new QueryString());
            _mockHttpContext.Setup(m => m.Request.Path).Returns("/path/to/resource1");
            _mockHttpContext.Setup(m => m.Response.Headers.Add("ETag", It.IsAny<StringValues>()));

            _filter.OnActionExecuting(_actionExecutingContext);

            _mockHttpContext.Verify(m => m.Response.Headers.Add("ETag", It.Is<StringValues>(v => v.Contains(eTag))));
        }

        [Fact]
        public void OnActionExecuting_WithMatchingIfNoneMatch_Responds304NotModified()
        {
            const string eTag = "03AC2E21CC0F0298899728A7648684A16B08DF1EA22AAD296AC916B50D6A9FE1";
            var jobDetails = new Dictionary<string, string>() { { "LastExecution", "2020-01-01 19:52:44" } };
            _mockStorageConnection.Setup(m => m.GetAllEntriesFromHash(
                $"recurring-job:{JobConfiguration.CrmSyncJobId}")).Returns(jobDetails);
            _mockHttpContext.Setup(m => m.Request.Headers["If-None-Match"]).Returns(eTag);
            _mockHttpContext.Setup(m => m.Request.Method).Returns("GET");
            _mockHttpContext.Setup(m => m.Request.QueryString).Returns(new QueryString());
            _mockHttpContext.Setup(m => m.Request.Path).Returns("/path/to/resource1");
            _mockHttpContext.Setup(m => m.Response.Headers.Add("ETag", It.IsAny<StringValues>()));

            _filter.OnActionExecuting(_actionExecutingContext);

            _actionExecutingContext.Result.Should().BeOfType<StatusCodeResult>().Which.StatusCode.Should()
                .Be((int) HttpStatusCode.NotModified);
            _mockHttpContext.Verify(m => m.Response.Headers.Add("ETag", It.Is<StringValues>(v => v.Contains(eTag))));
        }

        [Fact]
        public void OnActionExecuting_WithDifferentIfNoneMatch_RespondsWithResultAndETag()
        {
            const string eTag = "03AC2E21CC0F0298899728A7648684A16B08DF1EA22AAD296AC916B50D6A9FE1";
            var jobDetails = new Dictionary<string, string>() { { "LastExecution", "2020-01-01 19:52:44" } };
            _mockStorageConnection.Setup(m => m.GetAllEntriesFromHash(
                $"recurring-job:{JobConfiguration.CrmSyncJobId}")).Returns(jobDetails);
            _mockHttpContext.Setup(m => m.Request.Headers["If-None-Match"]).Returns("wrong-etag");
            _mockHttpContext.Setup(m => m.Request.Method).Returns("GET");
            _mockHttpContext.Setup(m => m.Request.QueryString).Returns(new QueryString());
            _mockHttpContext.Setup(m => m.Request.Path).Returns("/path/to/resource1");
            _mockHttpContext.Setup(m => m.Response.Headers.Add("ETag", It.IsAny<StringValues>()));

            _filter.OnActionExecuting(_actionExecutingContext);

            _actionExecutingContext.Result.Should().Be(_originalResult);
            _mockHttpContext.Verify(m => m.Response.Headers.Add("ETag", It.Is<StringValues>(v => v.Contains(eTag))));
        }

        [Fact]
        public void OnActionExecuting_SamePathDifferentQueryString_RespondsWithUniqueETag()
        {
            var eTags = new List<string>();
            _mockHttpContext.Setup(m => m.Request.Headers["If-None-Match"]).Returns<string>(null);
            _mockHttpContext.Setup(m => m.Request.Method).Returns("GET");
            _mockHttpContext.Setup(m => m.Request.QueryString).Returns(new QueryString());
            _mockHttpContext.SetupSequence(m => m.Request.Path)
                .Returns("/path/to/resource1?test=one")
                .Returns("/path/to/resource1?test=two");
            _mockHttpContext.Setup(m => m.Response.Headers.Add("ETag", It.IsAny<StringValues>()))
                .Callback<string, StringValues>((key, values) => eTags.AddRange(values.ToArray()));

            _filter.OnActionExecuting(_actionExecutingContext);
            _filter.OnActionExecuting(_actionExecutingContext);

            eTags.Distinct().Count().Should().Be(2);
        }

        [Fact]
        public void OnActionExecuted_BadResponse_ClearsETag()
        {
            const string eTag = "2B75BAF65CCDB4D32112CBEA0D66505CD5EC5A05D699EF8ED79169ED128D2F38";
            _mockHttpContext.Setup(m => m.Request.Headers["If-None-Match"]).Returns<string>(null);
            _mockHttpContext.Setup(m => m.Request.Method).Returns("GET");
            _mockHttpContext.Setup(m => m.Request.QueryString).Returns(new QueryString());
            _mockHttpContext.Setup(m => m.Request.Path).Returns("/path/to/resource1");
            _mockHttpContext.Setup(m => m.Response.Headers.Add("ETag", It.Is<StringValues>(v => v.Contains(eTag))));

            _filter.OnActionExecuting(_actionExecutingContext);

            _mockHttpContext.Setup(m => m.Response.StatusCode)
                .Returns((int)HttpStatusCode.InternalServerError);

            _filter.OnActionExecuted(_actionExecutedContext);

            _mockHttpContext.Verify(m => m.Response.Headers.Remove("ETag"));
        }

        [Fact]
        public void OnActionExecuted_OkResponse_RetainsETag()
        {
            const string eTag = "2B75BAF65CCDB4D32112CBEA0D66505CD5EC5A05D699EF8ED79169ED128D2F38";
            _mockHttpContext.Setup(m => m.Request.Headers["If-None-Match"]).Returns<string>(null);
            _mockHttpContext.Setup(m => m.Request.Method).Returns("GET");
            _mockHttpContext.Setup(m => m.Request.QueryString).Returns(new QueryString());
            _mockHttpContext.Setup(m => m.Request.Path).Returns("/path/to/resource1");
            _mockHttpContext.Setup(m => m.Response.Headers.Add("ETag", It.Is<StringValues>(v => v.Contains(eTag))));

            _filter.OnActionExecuting(_actionExecutingContext);

            _mockHttpContext.Setup(m => m.Response.StatusCode)
                .Returns((int)HttpStatusCode.OK);

            _filter.OnActionExecuted(_actionExecutedContext);

            _mockHttpContext.Verify(m => m.Response.Headers.Remove("ETag"), Times.Never);
        }
    }
}
