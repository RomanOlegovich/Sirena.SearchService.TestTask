namespace Sirena.SearchService.Infrastructure.InfrProviderOne;

// HTTP POST http://provider-one/api/v1/search

public class ProviderOneSearchRequest
{
    // Mandatory
    // Start point of route, e.g. Moscow 
    public string From { get; set; }

    // Mandatory
    // End point of route, e.g. Sochi
    public string To { get; set; }

    // Mandatory
    // Start date of route
    public DateTime DateFrom { get; set; }

    // Optional
    // End date of route
    public DateTime? DateTo { get; set; }

    // Optional
    // Maximum price of route
    public decimal? MaxPrice { get; set; }
}