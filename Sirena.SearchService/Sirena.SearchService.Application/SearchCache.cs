using Sirena.SearchService.Application.Contracts;
using Sirena.SearchService.Application.Contracts.Providers;
using Sirena.SearchService.Application.Contracts.PublicApi;

namespace Sirena.SearchService.Application
{
    public class SearchCache : ISearchCache
    {
        private readonly ICache _cache;

        public SearchCache(ICache cache)
        {
            _cache = cache;
        }

        private static string GetRoutesKey(ProviderSearchRequest request, ISearchServiceProvider provider)
        {
            var requestUuid = request.GetUuid();
            return $"request-{requestUuid}-provider-{provider.GetUniqueName()}";
        }

        private static string GetRouteKey(Guid routeId)
            => $"routeid-{routeId}";

        public Task<Route[]?> GetRoutesAsync(ISearchServiceProvider provider, ProviderSearchRequest request, CancellationToken cancellation)
        {
            var cacheKey = GetRoutesKey(request, provider);
            return _cache.Get<Route[]?>(cacheKey, cancellation);
        }

        public async Task SetRoutesAsync(ISearchServiceProvider provider, ProviderSearchRequest request, Route[] routes, CancellationToken cancellation)
        {
            if (routes.Length > 0)
            {
                var cacheKey = GetRoutesKey(request, provider);
                var cacheExpireAt = routes.Min(x => x.TimeLimit);
                await _cache.Set(cacheKey, routes, cancellation, cacheExpireAt);

                await CacheRoutesAsync(routes, cancellation);
            }
        }

        public async Task CacheRoutesAsync(Route[] routes, CancellationToken cancellation)
        {
            if (routes != null)
            {
                foreach (var route in routes)
                {
                    var cacheKey = GetRouteKey(route.Id);
                    await _cache.Set(cacheKey, route, cancellation, route.TimeLimit);
                }
            }
        }

        public Task<Route?> GetRouteAsync(Guid routeId, ISearchServiceProvider provider, CancellationToken cancellation)
        {
            var cacheKey = GetRouteKey(routeId);
            return _cache.Get<Route?>(cacheKey, cancellation);
        }
    }
}
