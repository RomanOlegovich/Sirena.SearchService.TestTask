namespace Sirena.SearchService.Application.Contracts
{
    public interface ICache
    {
        public Task<T> Get<T>(object key, CancellationToken cancellation);
        public Task Set<T>(object key, T value, CancellationToken cancellation, DateTime? expireAt = null);
    }
}
