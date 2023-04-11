using System.Net.Mime;
using System.Text;
using System.Text.Json;


namespace Sirena.SearchService.Application.Contracts.Providers
{
    public abstract class SearchServiceProviderBase<TInternalRequest, TInternalResponse> : ISearchServiceProvider
    {
        private readonly HttpClient _httpClient;

        public SearchServiceProviderBase(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public abstract string GetUniqueName();

        #region Search

        /// <summary>
        /// 1) Map <see cref="ProviderSearchRequest"/> to <see cref="TInternalRequest"/>;
        /// 2) Send POST request to <see cref="GetSearchUrl"/>
        /// 3) Map <see cref="TInternalResponse"/> to <see cref="ProviderRoute[]"/>;
        /// </summary>
        public async Task<ProviderRoute[]> SearchAsync(ProviderSearchRequest request, CancellationToken cancellationToken)
        {
            var providerRequest = MapToIntrnalRequest(request);
            var providerResponse = await SearchHttpAsync(providerRequest, cancellationToken);
            return MapInternalResponseToRoutes(providerResponse, request);
        }

        protected abstract TInternalRequest MapToIntrnalRequest(ProviderSearchRequest request);

        protected abstract ProviderRoute[] MapInternalResponseToRoutes(TInternalResponse response, ProviderSearchRequest request);

        private async Task<TInternalResponse> SearchHttpAsync(TInternalRequest request, CancellationToken cancellationToken)
        {
            var httpRequest = new StringContent(
                content: JsonSerializer.Serialize(request),
                encoding: Encoding.UTF8,
                mediaType: MediaTypeNames.Application.Json);

            var httpResponse = await _httpClient.PostAsync(GetSearchUrl(), httpRequest, cancellationToken);

            if (!httpResponse.IsSuccessStatusCode)
                throw new HttpRequestException(httpResponse.StatusCode.ToString());

            return JsonSerializer.Deserialize<TInternalResponse>(await httpResponse.Content.ReadAsStringAsync())!;
        }

        /// <summary>        
        /// Must return a url to send POST request to search routes
        /// </summary>
        protected abstract string GetSearchUrl();

        #endregion Search

        #region IsAvailable

        /// <summary>        
        /// 1) Send GET request to <see cref="GetPingUrl"/>
        /// 2) Return true if http response code is 200 otherwise false;
        /// </summary>
        public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken)
        {
            try
            {
                return await IsAvailableHttpAsync(cancellationToken);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>        
        /// Must return a url to check provider availability
        /// </summary>
        protected abstract string GetPingUrl();

        private async Task<bool> IsAvailableHttpAsync(CancellationToken cancellationToken)
        {
            var httpResponse = await _httpClient.GetAsync(GetPingUrl(), cancellationToken);
            return httpResponse.StatusCode == System.Net.HttpStatusCode.OK;
        }

        #endregion IsAvailable
    }
}
