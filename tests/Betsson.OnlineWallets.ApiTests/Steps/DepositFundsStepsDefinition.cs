using Betsson.OnlineWallets.Web.Models;
using Betsson.OnlineWallets.Web.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RestSharp;
using System.Net;
using TechTalk.SpecFlow;

namespace Betsson.OnlineWallets.ApiTests.Steps
{
    [Binding]
    public class DepositFundsStepsDefinition
    {
        private readonly IApiClient _apiClient;
        private ScenarioContext _scenarioContext;
        private DepositRequest _depositRequest;  
        private RestResponse _response;

        public DepositFundsStepsDefinition(IApiClient apiClient, ScenarioContext scenarioContext)
        {
            _apiClient = apiClient;
            _scenarioContext = scenarioContext;
        }

        [Given(@"the API endpoint for depositing funds is '(.*)'")]
        public void GivenTheAPIEndpointForDepositingFundsIs(string endPoint)
        {
            _scenarioContext.Add("end_point", endPoint);
        }

        [Given(@"I check my current balance before transaction")]
        public async Task GivenICheckMyCurrentBalanceBeforeTransaction()
        {
            var balanceBefore = await _apiClient.GetAsync<BalanceResponse>("/balance");
            _scenarioContext.Add("balance_before", balanceBefore.Amount);
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

        [Then(@"the response status code should be '(.*)'")]
        public void ThenTheResponseStatusCodeShouldBe(int statusCode)
        {
            _response.StatusCode.Should().Be((HttpStatusCode)statusCode);            
        }

        [Then(@"the balance should be updated to reflect the transaction")]
        public async Task ThenTheBalanceShouldBeUpdatedToReflectThe()
        {
            // Get current balance after transaction
            var balanceResponse = await _apiClient.GetAsync<BalanceResponse>("/balance");
            
            //Get all the values to be compare
            var newBalance = balanceResponse.Amount;
            var balanceBefore = _scenarioContext.Get<decimal>("balance_before");
            var transactionResponseAmount = _scenarioContext.Get<BalanceResponse>("response_data");

            newBalance.Should().Be(balanceBefore + _depositRequest.Amount);
            transactionResponseAmount.Amount.Should().Be(newBalance);
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

        [Then(@"the balance should not be updated")]
        public async Task ThenTheBalanceShouldNotBeUpdated()
        {
            var currentBalance = await _apiClient.GetAsync<BalanceResponse>("/balance");
            var balanceBefore = _scenarioContext.Get<decimal>("balance_before");
            balanceBefore.Should().Be(currentBalance.Amount);
        }

        
            
        
    }
}
