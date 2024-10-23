using Betsson.OnlineWallets.ApiTests.Config;
using Betsson.OnlineWallets.Web.Models;
using Betsson.OnlineWallets.Web.Services;
using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Betsson.OnlineWallets.ApiTests.Steps.WalletTransactions
{
    public class DepositSteps : WalletBaseSteps
    {
        public DepositSteps(IApiClient apiClient, ScenarioContext scenarioContext)
            : base(apiClient, scenarioContext)
        {
        }

        [When("I send a deposit POST request with the payload:")]
        public async Task WhenISendAPostRequestToWithThePayload(string transaction, Table table)
        {
            //Get the payload from feature file
            var amount = decimal.Parse(table.Rows[0]["amount"]);
            _depositRequest = new DepositRequest { Amount = amount };

            //Make Deposit
            var endPoint = _scenarioContext.Get<string>(ScenarioContextKeys.EndPoint);
            _response = await _apiClient.PostAsync(endPoint, _depositRequest);

            //Save The Result Content
            var responseData = JsonConvert.DeserializeObject<BalanceResponse>(_response.Content);
            _scenarioContext.Add("response_data", responseData);
        }
    }
}
