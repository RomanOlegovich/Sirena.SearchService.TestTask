namespace Sirena.SearchService.Application.Contracts.PublicApi;

public class SearchResponse
{
    // Mandatory
    // Array of routes
    public Route[] Routes { get; set; }

    // Mandatory
    // The cheapest route
    public decimal MinPrice { get; set; }

    // Mandatory
    // Most expensive route
    public decimal MaxPrice { get; set; }

    // Mandatory
    // The fastest route
    public int MinMinutesRoute { get; set; }

    // Mandatory
    // The longest route
    public int MaxMinutesRoute { get; set; }


    public static SearchResponse Create(IEnumerable<Route> routes)
    {
        var searchResponse = new SearchResponse();

        foreach (var route in routes)
        {
            searchResponse.Routes.Append(route);

            searchResponse.MinPrice = Math.Min(searchResponse.MinPrice, route.Price);
            searchResponse.MaxPrice = Math.Min(searchResponse.MaxPrice, route.Price);

            var routeMinutes = (route.OriginDateTime - route.DestinationDateTime).Minutes;
            searchResponse.MinMinutesRoute = Math.Min(searchResponse.MinMinutesRoute, routeMinutes);
            searchResponse.MaxMinutesRoute = Math.Min(searchResponse.MaxMinutesRoute, routeMinutes);
        }

        return searchResponse;
    }
}
