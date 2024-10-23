using Betsson.OnlineWallets.Web.Models;
using Betsson.OnlineWallets.Web.Services;
using Betsson.OnlineWallets.ApiTests.Config;
using FluentAssertions;
using Newtonsoft.Json;
using TechTalk.SpecFlow;

namespace Betsson.OnlineWallets.ApiTests.Steps.WalletTransactions
{
    [Binding]
    public class BalanceSteps : WalletBaseSteps
    {
        public BalanceSteps(IApiClient apiClient, ScenarioContext scenarioContext)
            : base(apiClient, scenarioContext)
        {
        }

        [Given(@"I check my current balance before transaction")]
        public async Task GivenICheckMyCurrentBalanceBeforeTransaction()
        {
            var balanceBefore = await _apiClient.GetAsync<BalanceResponse>("/balance");
            _scenarioContext.Add("balance_before", balanceBefore.Amount);
        }

        [Given(@"the user has a balance higher than '(.*)'")]
        public async Task GivenTheUserHasABalanceThan(decimal amount)
        {
            var balanceBefore = _scenarioContext.Get<decimal>(ScenarioContextKeys.InitialBalance);

            if (balanceBefore <= amount)
            {
                _depositRequest = new DepositRequest { Amount = amount + 20.00m };

                try
                {
                    _response = await _apiClient.PostAsync("/deposit", _depositRequest);
                    var responseData = JsonConvert.DeserializeObject<BalanceResponse>(_response.Content);
                    _scenarioContext.Set(responseData.Amount, ScenarioContextKeys.InitialBalance);
                }
                catch (Exception ex)
                {
                    throw new ($"Unable to increase the balance: {ex.Message}");
                }
            }
        }

        [When(@"I send a GET request to to get the balance")]
        public async Task WhenISendAGETRequestTo()
        {
            var endPoint = _scenarioContext.Get<string>(ScenarioContextKeys.EndPoint);
            _response = await _apiClient.GetAsync(endPoint);

            var responseData = JsonConvert.DeserializeObject<BalanceResponse>(_response.Content);
            _scenarioContext.Add(ScenarioContextKeys.ResponseContent, responseData);
        }
        [Then(@"the balance should not be updated")]
        public async Task ThenTheBalanceShouldNotBeUpdated()
        {
            var currentBalance = await _apiClient.GetAsync<BalanceResponse>("/balance");
            var balanceBefore = _scenarioContext.Get<decimal>(ScenarioContextKeys.InitialBalance);

            currentBalance.Amount.Should().Be(balanceBefore, "the balance is not updated");
        }

        [Then(@"the new balance should be '(.*)'")]
        public async Task ThenTheNewBalanceShouldBe(decimal balance)
        {
            var currentBalance = await _apiClient.GetAsync<BalanceResponse>("/balance");
            currentBalance.Amount.Should().Be(balance);
        }

        [Then(@"the balance should be updated to reflect the '(.*)' transaction")]
        public async Task ThenTheBalanceShouldBeUpdatedToReflectThe(string transaction)
        {
            // Get current balance after transaction
            var balanceResponse = await _apiClient.GetAsync<BalanceResponse>("/balance");

            //Get all the values to be compare
            var newBalance = balanceResponse.Amount;
            var balanceBefore = _scenarioContext.Get<decimal>(ScenarioContextKeys.InitialBalance);
            var transactionResponseAmount = _scenarioContext.Get<BalanceResponse>(ScenarioContextKeys.ResponseContent);

            //Assertions
            transactionResponseAmount.Amount.Should().Be(newBalance);

            switch (transaction)
            {
                case "deposit":
                    newBalance.Should().Be(balanceBefore + _depositRequest.Amount);
                    break;

                case "withdraw":
                    newBalance.Should().Be(balanceBefore - _withdrawalRequest.Amount);
                    break;

                default:
                    throw new ArgumentOutOfRangeException($"There is not implementation for {transaction}");
            };
        }
    }
}
