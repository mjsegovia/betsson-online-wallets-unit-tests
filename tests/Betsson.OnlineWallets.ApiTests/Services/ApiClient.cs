using IconTestFramework.Core.Config;
using Newtonsoft.Json;
using RestSharp;
using System.Security.Policy;
using System.Text;

namespace Betsson.OnlineWallets.Web.Services
{
    public class ApiClient : IApiClient
    {
        private readonly RestClient _client;

        public ApiClient()
        {
            _client = new RestClient(new Uri(Configurator.BaseUrl));           
        }

        public async Task<RestResponse> GetAsync(string url)
        {
            var request = new RestRequest(url, Method.Get);
            var response = await _client.ExecuteAsync(request);

            HandleErrors(response);

            return response;
        }

        public async Task<T> GetAsync<T>(string url)
        {
            var request = new RestRequest(url, Method.Get);
            var response = await _client.ExecuteAsync(request);

            HandleErrors(response);

            var content = response.Content; 
              
            var responseData = JsonConvert.DeserializeObject<T>(content);
            
            return responseData;
        }

        public async Task<RestResponse> PostAsync<T>(string url, T payload) where T : class
        {
            var request = new RestRequest(url, Method.Post);
            request.AddJsonBody(payload);

            var response = await _client.ExecuteAsync(request);
           
            var content = response.Content;
            var responseData = JsonConvert.DeserializeObject<T>(content);            

            return response;
        }

        public async Task<string> GetResponseContent(RestResponse response)
            =>  response.Content ?? throw new HttpRequestException($"Response content is null for the request.");    

        //Private method to handle errors
        private void HandleErrors(RestResponse response)
        {
            if (!response.IsSuccessful)
                throw new SystemException($"Error: {response.StatusCode} - {response.ErrorMessage}");
        }
    }
}
