using Betsson.OnlineWallets.Web.Models;
using Betsson.OnlineWallets.Web.Services;
using Newtonsoft.Json;
using RestSharp;
using TechTalk.SpecFlow;

namespace Betsson.OnlineWallets.ApiTests.Steps
{
    [Binding]
    public abstract class WalletBaseSteps
    {
        protected readonly IApiClient _apiClient;
        protected ScenarioContext _scenarioContext;
        protected DepositRequest _depositRequest;
        protected WithdrawalRequest _withdrawalRequest;
        protected RestResponse _response;

        public WalletBaseSteps(IApiClient apiClient, ScenarioContext scenarioContext)
        {
            _apiClient = apiClient;
            _scenarioContext = scenarioContext;
        } 
        
        protected ErrorResponse GetErrorResponse() => JsonConvert.DeserializeObject<ErrorResponse>(_response.Content);       
    }
}