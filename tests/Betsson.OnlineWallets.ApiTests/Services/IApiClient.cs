using RestSharp;

namespace Betsson.OnlineWallets.Web.Services
{
    public interface IApiClient
    {
        Task<RestResponse> GetAsync(string url);

        Task<T> GetAsync<T>(string url);

        Task<RestResponse> PostAsync<T>(string url, T payload) where T : class;

        Task<string> GetResponseContent(RestResponse response);
    }
}
