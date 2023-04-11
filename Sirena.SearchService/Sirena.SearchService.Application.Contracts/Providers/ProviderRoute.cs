using System.Security.Cryptography;
using System.Text;

namespace Sirena.SearchService.Application.Contracts.Providers;

public class ProviderRoute
{
    public ProviderRoute(string origin,
                         string destination,
                         DateTime originDateTime,
                         DateTime destinationDateTime,
                         decimal price,
                         DateTime timeLimit)
    {
        Origin = origin ?? throw new ArgumentNullException();
        Destination = destination ?? throw new ArgumentNullException();
        OriginDateTime = originDateTime;
        DestinationDateTime = destinationDateTime;
        Price = price;
        TimeLimit = timeLimit;
    }

    // Mandatory
    // Start point of route
    public string Origin { get; protected set; }

    // Mandatory
    // End point of route
    public string Destination { get; protected set; }

    // Mandatory
    // Start date of route
    public DateTime OriginDateTime { get; protected set; }

    // Mandatory
    // End date of route
    public DateTime DestinationDateTime { get; protected set; }

    // Mandatory
    // Price of route
    public decimal Price { get; protected set; }

    // Mandatory
    // Timelimit. After it expires, route became not actual
    public DateTime TimeLimit { get; protected set; }

    /// <summary>
    /// Generate stable uuid via md5 of all fields
    /// </summary>
    public Guid GetUuid()
    {
        using var md5 = MD5.Create();

        md5.ComputeHash(Encoding.UTF8.GetBytes(Origin));

        md5.ComputeHash(Encoding.UTF8.GetBytes(Destination));

        md5.ComputeHash(BitConverter.GetBytes(OriginDateTime.Ticks));

        md5.ComputeHash(BitConverter.GetBytes(DestinationDateTime.Ticks));

        var priceBytes = decimal.GetBits(Price).SelectMany(integer => BitConverter.GetBytes(integer)).ToArray();
        md5.ComputeHash(priceBytes);

        return new Guid(md5.Hash!);
    }
}