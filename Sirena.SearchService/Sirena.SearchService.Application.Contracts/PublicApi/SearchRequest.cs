using System.ComponentModel.DataAnnotations;

namespace Sirena.SearchService.Application.Contracts.PublicApi;

public class SearchRequest
{
    // Mandatory
    // Start point of route, e.g. Moscow 
    [Required]
    public string Origin { get; set; }

    // Mandatory
    // End point of route, e.g. Sochi
    [Required]
    public string Destination { get; set; }

    // Mandatory
    // Start date of route
    [Required]
    public DateTime OriginDateTime { get; set; }

    // Optional
    public SearchFilters? Filters { get; set; }
}
