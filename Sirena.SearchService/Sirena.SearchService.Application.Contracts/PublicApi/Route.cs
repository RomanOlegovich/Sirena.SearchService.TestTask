namespace Sirena.SearchService.Application.Contracts.PublicApi;

public class Route
{
    protected Route() { }

    public Route(Guid id,
                 string origin,
                 string destination,
                 DateTime originDateTime,
                 DateTime destinationDateTime,
                 decimal price,
                 DateTime timeLimit)
    {
        Id = id;
        Origin = origin;
        Destination = destination;
        OriginDateTime = originDateTime;
        DestinationDateTime = destinationDateTime;
        Price = price;
        TimeLimit = timeLimit;
    }

    // Mandatory
    // Identifier of the whole route
    public Guid Id { get; protected set; }

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
}