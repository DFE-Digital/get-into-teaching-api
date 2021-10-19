using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl.Http;

namespace GetIntoTeachingApi.Services
{
    public class PaginatorClient<T> : IPaginatorClient<T>
    {
        private readonly IFlurlRequest _request;
        private bool _hasNext;
        private int _page;

        public bool HasNext
        {
            get
            {
                return _hasNext;
            }
        }

        public PaginatorClient(IFlurlRequest request)
        {
            _request = request;
            _hasNext = true;
            _page = 1;
        }

        public async Task<T> NextAsync()
        {
            var response = await _request
                .SetQueryParam("page", _page)
                .GetAsync();
            var headers = response.Headers;

            if (!headers.Contains("Total-Pages") || !headers.Contains("Current-Page"))
            {
                throw new KeyNotFoundException("Expected Total-Pages and Current-Page header keys");
            }

            var totalPages = headers.FirstOrDefault("Total-Pages");
            var currentPage = headers.FirstOrDefault("Current-Page");
            _hasNext = currentPage != totalPages;

            _page++;

            return await response.GetJsonAsync<T>();
        }
    }
}
