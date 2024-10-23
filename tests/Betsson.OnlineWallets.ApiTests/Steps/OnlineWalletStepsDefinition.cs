using Betsson.OnlineWallets.Models;
using Betsson.OnlineWallets.Web.Models;
using Betsson.OnlineWallets.Web.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RestSharp;
using System.Net;
using TechTalk.SpecFlow;
using Xunit.Sdk;

namespace Betsson.OnlineWallets.ApiTests.Steps
{
    [Binding]
    public class OnlineWalletStepsDefinition
    {
        private readonly IApiClient _apiClient;
        private ScenarioContext _scenarioContext;
        private DepositRequest _depositRequest;
        private WithdrawalRequest _withdrawalRequest;
        private RestResponse _response;

        public OnlineWalletStepsDefinition(IApiClient apiClient, ScenarioContext scenarioContext)
        {
            _apiClient = apiClient;
            _scenarioContext = scenarioContext;
        }

        [Given(@"the API endpoint for withdrawal funds is '(.*)'")]
        [Given(@"the API endpoint for depositing funds is '(.*)'")]
        [Given(@"the API endpoint to get balance is '(.*)'")]
        public void GivenTheAPIEndpointToGetBalanceIs(string endPoint)
        {
            _scenarioContext.Add("end_point", endPoint);
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
            var balanceBefore = _scenarioContext.Get<decimal>("balance_before");

            if (balanceBefore <= amount)
            {
                _depositRequest = new DepositRequest { Amount = amount + 20.00m };

                try
                {
                    _response = await _apiClient.PostAsync("/deposit", _depositRequest);
                    var responseData = JsonConvert.DeserializeObject<BalanceResponse>(_response.Content);
                    _scenarioContext.Set<decimal>(responseData.Amount, "balance_before");
                }
                catch (Exception ex)
                {
                    throw new BadHttpRequestException($"Unable to increase the balance: {ex.Message}");
                }
            }
        }

        [When("I send a POST request with the payload:")]
        public async Task WhenISendAPostRequestToWithThePayload(Table table)
        {
            var amount = decimal.Parse(table.Rows[0]["amount"]);
            _depositRequest = new DepositRequest { Amount = amount };

            var endPoint = _scenarioContext.Get<string>("end_point");

            _response = await _apiClient.PostAsync(endPoint, _depositRequest);
            var responseData = JsonConvert.DeserializeObject<BalanceResponse>(_response.Content);
            _scenarioContext.Add("response_data", responseData);
        }

        [When(@"I send a GET request to '(.*)'")]
        public async Task WhenISendAGETRequestTo(string p0)
        {
            var endPoint = _scenarioContext.Get<string>("end_point");
            _response = await _apiClient.GetAsync(endPoint);
            var responseData = JsonConvert.DeserializeObject<BalanceResponse>(_response.Content);
            _scenarioContext.Add("response_data", responseData);
        }

        [When(@"the user withdraws '(.*)'")]
        public async Task WhenTheUserWithdrawsUSD(decimal withdrawalAmount)
        {
            _withdrawalRequest = new WithdrawalRequest { Amount = withdrawalAmount };

            _response = await _apiClient.PostAsync("/withdraw", _withdrawalRequest);
            var responseData = JsonConvert.DeserializeObject<BalanceResponse>(_response.Content);
            _scenarioContext.Add("response_data", responseData);
        }

        [When(@"the user attempts to withdraw an amount greater than their current balance")]
        public async Task WhenTheUserAttemptsToWithdrawGreaterThanCurrentBalance()
        {
            var currentBalance = _scenarioContext.Get<decimal>("balance_before");

            _withdrawalRequest = new WithdrawalRequest { Amount = currentBalance + 20.00m };

            var endPoint = _scenarioContext.Get<string>("end_point");

            _response = await _apiClient.PostAsync(endPoint, _withdrawalRequest);
            var responseData = JsonConvert.DeserializeObject<BalanceResponse>(_response.Content);
            _scenarioContext.Add("response_data", responseData);
        }

        [When(@"the user withdraws all the funds from account")]
        public async Task WhenTheUserWithdrawsAllTheFundsFromAccount()
        {
            var currentBalance = _scenarioContext.Get<decimal>("balance_before");
            var withdrawalRequest = new Withdrawal { Amount = currentBalance };

            _response = await _apiClient.PostAsync("/withdraw", withdrawalRequest);
            var responseData = JsonConvert.DeserializeObject<BalanceResponse>(_response.Content);
            _scenarioContext.Add("response_data", responseData);
        }

        [Then(@"the response status code should be '(.*)'")]
        public void ThenTheResponseStatusCodeShouldBe(int statusCode)
        {
            _response.StatusCode.Should().Be((HttpStatusCode)statusCode);
        }

        [Then(@"the balance should be updated to reflect the '(.*)' transaction")]
        public async Task ThenTheBalanceShouldBeUpdatedToReflectThe(string transaction)
        {
            // Get current balance after transaction
            var balanceResponse = await _apiClient.GetAsync<BalanceResponse>("/balance");

            //Get all the values to be compare
            var newBalance = balanceResponse.Amount;
            var balanceBefore = _scenarioContext.Get<decimal>("balance_before");
            var transactionResponseAmount = _scenarioContext.Get<BalanceResponse>("response_data");

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

        [Then(@"the transacction should be successful with status '(.*)'")]
        public void ThenTheTransactionShouldBeSuccessfulWithStatus(string expectedStatus)
        {
            _response.IsSuccessful.Should().BeTrue();
            _response.ResponseStatus.ToString().Should().Be(expectedStatus);
        }

        [Then(@"the status description should be '(.*)'")]
        public void ThenTheResponseShoulBe(string responseCode)
        {
            _response.StatusDescription.ToString().Should().Be(responseCode);
        }

        [Then(@"the response status should be '(.*)'")]
        public void ThenTheErrorTitleIs(string status)
        {
            _response.ResponseStatus.ToString().Should().Be(status);
        }

        [Then(@"the response should contain an error message '(.*)'")]
        public void ThenTheResponseShouldContainAnErrorMessageMustBeGreaterThanOrEqualTo_(string errorMessage)
        {
            var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(_response.Content);
            string amountErrorMessage = errorResponse.Errors.ContainsKey("Amount")
                ? errorResponse.Errors["Amount"][0]
                : "No error message for Amount.";

            amountErrorMessage.Should().Be(errorMessage);
        }

        [Then(@"the exception type is '(.*)'")]
        public void ThenTheExceptionTypeIs(string ExceptionType)
        {
            var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(_response.Content);

            ExceptionType.Should().Be(errorResponse.Type);
        }

        [Then(@"the exception error message is '(.*)'")]
        public void ThenTheExceptionErrorMessageIs(string message)
        {
            var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(_response.Content);

            message.Should().Be(errorResponse.Title);
        }

        [Then(@"the balance should not be updated")]
        public async Task ThenTheBalanceShouldNotBeUpdated()
        {
            var currentBalance = await _apiClient.GetAsync<BalanceResponse>("/balance");
            var balanceBefore = _scenarioContext.Get<decimal>("balance_before");
            currentBalance.Amount.Should().Be(balanceBefore, "no transactios is made with negative amount");
        }        

        [Then(@"the new balance should be '(.*)'")]
        public async Task ThenTheNewBalanceShouldBe(decimal balance)
        {
            var currentBalance = await _apiClient.GetAsync<BalanceResponse>("/balance");
            currentBalance.Amount.Should().Be(balance);
        }
    }
}
