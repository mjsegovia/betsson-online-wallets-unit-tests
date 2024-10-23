using Betsson.OnlineWallets.ApiTests.Config;
using Betsson.OnlineWallets.Models;
using Betsson.OnlineWallets.Web.Models;
using Betsson.OnlineWallets.Web.Services;
using Newtonsoft.Json;
using TechTalk.SpecFlow;

namespace Betsson.OnlineWallets.ApiTests.Steps.WalletTransactions
{
    public class WithdrawalSteps : WalletBaseSteps
    {
        public WithdrawalSteps(IApiClient apiClient, ScenarioContext scenarioContext)
            : base(apiClient, scenarioContext)
        {
        }

        [When(@"the user withdraws '(.*)'")]
        public async Task WhenTheUserWithdrawsUSD(decimal withdrawalAmount)
        {
            _withdrawalRequest = new WithdrawalRequest { Amount = withdrawalAmount };

            _response = await _apiClient.PostAsync("/withdraw", _withdrawalRequest);
            var responseData = JsonConvert.DeserializeObject<BalanceResponse>(_response.Content);
            _scenarioContext.Add(ScenarioContextKeys.ResponseContent, responseData);
        }

        [When(@"the user attempts to withdraw an amount greater than their current balance")]
        public async Task WhenTheUserAttemptsToWithdrawGreaterThanCurrentBalance()
        {
            var currentBalance = _scenarioContext.Get<decimal>(ScenarioContextKeys.InitialBalance);

            _withdrawalRequest = new WithdrawalRequest { Amount = currentBalance + 20.00m };

            var endPoint = _scenarioContext.Get<string>("end_point");

            _response = await _apiClient.PostAsync(endPoint, _withdrawalRequest);
            var responseData = JsonConvert.DeserializeObject<BalanceResponse>(_response.Content);
            _scenarioContext.Add(ScenarioContextKeys.ResponseContent, responseData);
        }

        [When(@"the user withdraws all the funds from account")]
        public async Task WhenTheUserWithdrawsAllTheFundsFromAccount()
        {
            var currentBalance = _scenarioContext.Get<decimal>("balance_before");
            var withdrawalRequest = new Withdrawal { Amount = currentBalance };

            _response = await _apiClient.PostAsync("/withdraw", withdrawalRequest);
            var responseData = JsonConvert.DeserializeObject<BalanceResponse>(_response.Content);
            _scenarioContext.Add(ScenarioContextKeys.ResponseContent, responseData);
        }
    }
}
