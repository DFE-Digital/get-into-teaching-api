using System.Collections.Generic;
using FluentAssertions;
using Flurl;
using Flurl.Http;
using Flurl.Http.Testing;
using GetIntoTeachingApi.Services;
using Newtonsoft.Json;
using Xunit;

namespace GetIntoTeachingApiTests.Services
{
    public class PaginatorClientTests
    {
        private readonly IPaginatorClient<string> _paginator;

        public PaginatorClientTests()
        {
            var request = "https://test.com"
               .AppendPathSegment("data")
               .WithOAuthBearerToken("abc-123");

            _paginator = new PaginatorClient<string>(request);
        }

        [Fact]
        public void HasNext_BeforeRetrievingFirstPage_IsTrue()
        {
            _paginator.HasNext.Should().BeTrue();
        }

        [Fact]
        public async void HasNext_WhenThereAreMorePages_IsTrue()
        {
            using var httpTest = new HttpTest();
            MockResponse(httpTest, "page 1 data", 1, 2);
            MockResponse(httpTest, "page 2 data", 2, 2);

            await _paginator.NextAsync();

            _paginator.HasNext.Should().BeTrue();
        }

        [Fact]
        public async void HasNext_WhenThereAreNoMorePages_IsFalse()
        {
            using var httpTest = new HttpTest();
            MockResponse(httpTest, "page 1 data", 1, 2);
            MockResponse(httpTest, "page 2 data", 2, 2);

            await _paginator.NextAsync();
            await _paginator.NextAsync();

            _paginator.HasNext.Should().BeFalse();
        }

        [Fact]
        public async void NextAsync_FirstCall_RetrievesPage1()
        {
            using var httpTest = new HttpTest();
            MockResponse(httpTest, "page 1 data");

            var response = await _paginator.NextAsync();

            response.Should().Be("page 1 data");
        }

        [Fact]
        public async void NextAsync_SecondCall_RetrievesPage2()
        {
            using var httpTest = new HttpTest();
            MockResponse(httpTest, "page 1 data", 1, 2);
            MockResponse(httpTest, "page 2 data", 2, 2);

            await _paginator.NextAsync();
            var response = await _paginator.NextAsync();

            response.Should().Be("page 2 data");
        }

        [Fact]
        public void NextAsync_ResponseIncorrectHeaders_Throws()
        {
            using var httpTest = new HttpTest();
            MockResponse(httpTest, "page 1 data", null, null);

            _paginator.Invoking(p => p.NextAsync())
                .Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("Expected Total-Pages and Current-Page header keys");
        }

        private void MockResponse(HttpTest httpTest, string response, int? page = 1, int? totalPages = 1)
        {
            var json = JsonConvert.SerializeObject(response);
            var headers = new Dictionary<string, int?>() { { "Total-Pages", totalPages }, { "Current-Page", page } };

            httpTest
                    .ForCallsTo("https://test.com/data")
                    .WithVerb("GET")
                    .WithQueryParam("page", page)
                    .WithHeader("Authorization", $"Bearer abc-123")
                    .RespondWith(json, 200, headers);
        }
    }
}
