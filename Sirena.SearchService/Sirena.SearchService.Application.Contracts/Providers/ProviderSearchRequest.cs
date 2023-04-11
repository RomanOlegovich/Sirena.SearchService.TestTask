using System.Security.Cryptography;
using System.Text;

namespace Sirena.SearchService.Application.Contracts.Providers;

public class ProviderSearchRequest
{
    protected ProviderSearchRequest() { }

    public ProviderSearchRequest(string origin,
                                 string destination,
                                 DateTime originDateTime,
                                 DateTime? destinationDateTime,
                                 decimal? maxPrice,
                                 DateTime? minTimeLimit)
    {
        Origin = origin ?? throw new ArgumentNullException();
        Destination = destination ?? throw new ArgumentNullException();
        OriginDateTime = originDateTime;
        DestinationDateTime = destinationDateTime;
        MaxPrice = maxPrice;
        MinTimeLimit = minTimeLimit;
    }

    // Mandatory
    // Start point of route, e.g. Moscow 
    public string Origin { get; protected set; }

    // Mandatory
    // End point of route, e.g. Sochi
    public string Destination { get; protected set; }

    // Mandatory
    // Start date of route
    public DateTime OriginDateTime { get; protected set; }

    // Optional
    // End date of route
    public DateTime? DestinationDateTime { get; protected set; }

    // Optional
    // Maximum price of route
    public decimal? MaxPrice { get; protected set; }

    // Optional
    // Minimum value of timelimit for route
    public DateTime? MinTimeLimit { get; protected set; }

    /// <summary>
    /// Generate stable uuid via md5 of all fields
    /// </summary>
    public Guid GetUuid()
    {
        using var md5 = MD5.Create();

        md5.ComputeHash(Encoding.UTF8.GetBytes(Origin));

        md5.ComputeHash(Encoding.UTF8.GetBytes(Destination));

        md5.ComputeHash(BitConverter.GetBytes(OriginDateTime.Ticks));

        if (DestinationDateTime.HasValue)
            md5.ComputeHash(BitConverter.GetBytes(DestinationDateTime.Value.Ticks));

        if (MaxPrice.HasValue)
        {
            var priceBytes = decimal.GetBits(MaxPrice.Value).SelectMany(integer => BitConverter.GetBytes(integer)).ToArray();
            var hashBytes = md5.ComputeHash(priceBytes);
        }

        return new Guid(md5.Hash!);
    }
}
