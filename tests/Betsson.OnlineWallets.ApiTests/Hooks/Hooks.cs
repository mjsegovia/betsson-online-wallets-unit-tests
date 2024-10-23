using Betsson.OnlineWallets.Web.Services;
using BoDi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Betsson.OnlineWallets.ApiTests.Hooks
{
    [Binding]
    public class Hooks
    {
        [BeforeScenario]
        public void RegisterDependencies(IObjectContainer container)
        {
            // Register IApiClient with its implementation
            container.RegisterTypeAs<ApiClient, IApiClient>();
        }
    }
}