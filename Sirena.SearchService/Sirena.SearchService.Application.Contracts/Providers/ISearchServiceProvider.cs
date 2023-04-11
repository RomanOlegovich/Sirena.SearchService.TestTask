namespace Sirena.SearchService.Application.Contracts.Providers;

public interface ISearchServiceProvider
{
    string GetUniqueName();
    Task<ProviderRoute[]> SearchAsync(ProviderSearchRequest request, CancellationToken cancellationToken);
    Task<bool> IsAvailableAsync(CancellationToken cancellationToken);
}
