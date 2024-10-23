using Betsson.OnlineWallets.ApiTests.Config;
using Betsson.OnlineWallets.Web.Models;
using Betsson.OnlineWallets.Web.Services;
using FluentAssertions;
using Newtonsoft.Json;
using System.Net;
using TechTalk.SpecFlow;

namespace Betsson.OnlineWallets.ApiTests.Steps
{
    public class CommonApiSteps : WalletBaseSteps 
    {
        public CommonApiSteps(IApiClient apiClient, ScenarioContext scenarioContext) : base(apiClient, scenarioContext)
        {
        }

        [Given(@"the API endpoint for withdrawal funds is '(.*)'")]
        [Given(@"the API endpoint for depositing funds is '(.*)'")]
        [Given(@"the API endpoint to get balance is '(.*)'")]
        public void GivenTheAPIEndpointIs(string endPoint)
        {
            _scenarioContext.Add(ScenarioContextKeys.EndPoint, endPoint);
        }

        [Then(@"the response status code should be '(.*)'")]
        public void ThenTheResponseStatusCodeShouldBe(int statusCode)
        {
            _response.StatusCode.Should().Be((HttpStatusCode)statusCode);
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
            var errorResponse = GetErrorResponse();
            string amountErrorMessage = errorResponse.Errors.ContainsKey("Amount")
                ? errorResponse.Errors["Amount"][0]
                : "No error message for Amount.";

            amountErrorMessage.Should().Be(errorMessage);
        }

        [Then(@"the exception type is '(.*)'")]
        public void ThenTheExceptionTypeIs(string ExceptionType)
        {
            ExceptionType.Should().Be(GetErrorResponse().Type);
        }

        [Then(@"the exception error message is '(.*)'")]
        public void ThenTheExceptionErrorMessageIs(string message)
        {
            message.Should().Be(GetErrorResponse().Title);
        }
    }
}
