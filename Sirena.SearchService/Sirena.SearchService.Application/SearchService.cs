using Microsoft.Extensions.Logging;
using Sirena.SearchService.Application.Contracts;
using Sirena.SearchService.Application.Contracts.Providers;
using Sirena.SearchService.Application.Contracts.PublicApi;
using System.Collections.Concurrent;

namespace Sirena.SearchService.Application
{
    public class SearchService : ISearchService
    {
        private readonly IList<ISearchServiceProvider> _providers;
        private readonly ISearchCache _searchCache;
        private readonly ILogger<SearchService> _logger;

        public SearchService(IList<ISearchServiceProvider> providers,
                             ISearchCache searchCache,
                             ILogger<SearchService> logger)
        {
            _providers = providers;
            _searchCache = searchCache;
            _logger = logger;
        }

        /// <summary>
        /// Return true if all providers are available
        /// </summary>
        public async Task<bool> IsAvailableAsync(CancellationToken cancellation)
        {
            var healthCheckTasks = _providers.Select(provider => provider.IsAvailableAsync(cancellation));
            var healthChecks = await Task.WhenAll(healthCheckTasks);
            return healthChecks.All(isAvailable => isAvailable);
        }

        public async Task<SearchResponse> SearchAsync(SearchRequest request, CancellationToken cancellation)
        {
            var routes = await SearchInternalAsync(request, cancellation);
            return SearchResponse.Create(routes);
        }

        /// <summary>
        /// Retrieve data from all providers
        /// </summary>
        private async Task<IEnumerable<Route>> SearchInternalAsync(SearchRequest request, CancellationToken cancellation)
        {
            var routesBag = new ConcurrentBag<Route[]>();

            await Parallel.ForEachAsync(_providers, cancellation, async (provider, cancellation) =>
            {
                try
                {
                    var providerRoutes = await GetProviderRoutesAsync(provider, request, cancellation);
                    if (providerRoutes?.Length > 0)
                    {
                        routesBag.Add(providerRoutes);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during provider {providerName} search.", provider.GetUniqueName());
                }
            });

            return routesBag.SelectMany(r => r);
        }

        /// <summary>
        /// Return cache if SearchRequest->Filters->OnlyCached is true and cache exists;
        /// otherwise retrieve routes from provider and set to cache.
        /// </summary>
        private async Task<Route[]> GetProviderRoutesAsync(ISearchServiceProvider provider, SearchRequest request, CancellationToken cancellation)
        {
            var providerRequest = new ProviderSearchRequest(
                origin: request.Origin,
                destination: request.Destination,
                destinationDateTime: request.Filters?.DestinationDateTime,
                originDateTime: request.OriginDateTime,
                maxPrice: request.Filters?.MaxPrice,
                minTimeLimit: request.Filters?.MinTimeLimit);

            // Return cached if OnlyCached and cache exists
            if (request.Filters?.OnlyCached == true)
            {
                var cached = (await _searchCache.GetRoutesAsync(provider, providerRequest, cancellation)) ?? new Route[0];
                if (cached.Length > 0)
                    return cached;
            }

            cancellation.ThrowIfCancellationRequested();

            // Retrieve data from provider and set routes cache
            var providerRoutes = await provider.SearchAsync(providerRequest, cancellation);
            var routes = providerRoutes.Select(route => new Route(
                id: route.GetUuid(),
                origin: route.Origin,
                destination: route.Destination,
                originDateTime: route.OriginDateTime,
                destinationDateTime: route.DestinationDateTime,
                price: route.Price,
                timeLimit: route.TimeLimit)).ToArray();
            await _searchCache.SetRoutesAsync(provider, providerRequest, routes, cancellation);

            return routes;
        }
    }
}
