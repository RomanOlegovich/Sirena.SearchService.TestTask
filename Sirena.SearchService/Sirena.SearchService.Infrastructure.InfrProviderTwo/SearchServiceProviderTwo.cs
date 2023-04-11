using Sirena.SearchService.Application.Contracts.Providers;

namespace Sirena.SearchService.Infrastructure.InfrProviderTwo
{
    public class SearchServiceProviderTwo : SearchServiceProviderBase<ProviderTwoSearchRequest, ProviderTwoSearchResponse>
    {
        public SearchServiceProviderTwo(HttpClient httpClient) : base(httpClient)
        {
        }

        public override string GetUniqueName()
        {
            return nameof(SearchServiceProviderTwo);
        }

        protected override string GetPingUrl() => "http://provider-two/api/v1/ping";

        protected override string GetSearchUrl() => "http://provider-two/api/v1/search";

        protected override ProviderTwoSearchRequest MapToIntrnalRequest(ProviderSearchRequest request)
        {
            return new ProviderTwoSearchRequest()
            {
                Departure = request.Origin,
                Arrival = request.Destination,
                DepartureDate = request.OriginDateTime,
                MinTimeLimit = request.MinTimeLimit,
            };
        }

        protected override ProviderRoute[] MapInternalResponseToRoutes(ProviderTwoSearchResponse response, ProviderSearchRequest request)
        {
            var filtered = ApplyUnsupportedFilters(response.Routes, request);
            var routes = filtered.Select(r => new ProviderRoute(
                origin: r.Departure.Point,
                destination: r.Arrival.Point,
                originDateTime: r.Departure.Date,
                destinationDateTime: r.Arrival.Date,
                price: r.Price,
                timeLimit: r.TimeLimit)).ToArray();

            return routes;
        }

        private ProviderTwoRoute[] ApplyUnsupportedFilters(ProviderTwoRoute[] routes, ProviderSearchRequest request)
        {
            var filtered = routes.AsEnumerable();

            if (request.DestinationDateTime.HasValue)
            {
                var destinationDate = request.DestinationDateTime.Value.Date;
                filtered = filtered.Where(route => route.Arrival.Date.Date == destinationDate);
            }

            if (request.MaxPrice.HasValue)
                filtered = filtered.Where(route => route.Price <= request.MaxPrice.Value);

            return filtered.ToArray();
        }
    }
}
