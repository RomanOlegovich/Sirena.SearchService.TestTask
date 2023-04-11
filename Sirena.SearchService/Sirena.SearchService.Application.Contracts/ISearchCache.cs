using Sirena.SearchService.Application.Contracts.Providers;
using Sirena.SearchService.Application.Contracts.PublicApi;

namespace Sirena.SearchService.Application.Contracts
{
    public interface ISearchCache
    {
        public Task<Route[]?> GetRoutesAsync(ISearchServiceProvider provider, ProviderSearchRequest request, CancellationToken cancellation);
        public Task SetRoutesAsync(ISearchServiceProvider provider, ProviderSearchRequest request, Route[] value, CancellationToken cancellation);
        public Task<Route?> GetRouteAsync(Guid routeId, ISearchServiceProvider provider, CancellationToken cancellation);
    }
}
