using Sirena.SearchService.Application.Contracts.Providers;

namespace Sirena.SearchService.Infrastructure.InfrProviderOne
{
    public class SearchServiceProviderOne : SearchServiceProviderBase<ProviderOneSearchRequest, ProviderOneSearchResponse>
    {
        public SearchServiceProviderOne(HttpClient httpClient) : base(httpClient)
        {
        }

        public override string GetUniqueName()
        {
            return nameof(SearchServiceProviderOne);
        }

        protected override string GetPingUrl() => "http://provider-one/api/v1/ping";

        protected override string GetSearchUrl() => "http://provider-one/api/v1/search";

        protected override ProviderOneSearchRequest MapToIntrnalRequest(ProviderSearchRequest request)
        {
            return new ProviderOneSearchRequest()
            {
                From = request.Origin,
                To = request.Destination,
                DateFrom = request.OriginDateTime,
                DateTo = request.DestinationDateTime,
                MaxPrice = request.MaxPrice,
            };
        }

        protected override ProviderRoute[] MapInternalResponseToRoutes(ProviderOneSearchResponse response, ProviderSearchRequest request)
        {
            var filtered = ApplyUnsupportedFilters(response.Routes, request);
            return filtered.Select(r => new ProviderRoute(
                origin: r.From,
                destination: r.To,
                originDateTime: r.DateFrom,
                destinationDateTime: r.DateTo,
                price: r.Price,
                timeLimit: r.TimeLimit)).ToArray();
        }

        private ProviderOneRoute[] ApplyUnsupportedFilters(ProviderOneRoute[] routes, ProviderSearchRequest request)
        {
            var filtered = routes.AsEnumerable();

            if(request.MinTimeLimit.HasValue)
                filtered = filtered.Where(route => route.TimeLimit > request.MinTimeLimit.Value);

            return filtered.ToArray();
        }
    }
}
