using Microsoft.AspNetCore.Mvc;
using Sirena.SearchService.Application.Contracts.PublicApi;

namespace Sirena.SearchService.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet]
        public Task IsAvailableAsync(CancellationToken cancellation)
        {
            return _searchService.IsAvailableAsync(cancellation);
        }

        [HttpPost]
        public Task SearchAsync([FromBody] SearchRequest request, CancellationToken cancellation)
        {
            return _searchService.SearchAsync(request, cancellation);
        }
    }
}