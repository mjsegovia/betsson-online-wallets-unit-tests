using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Betsson.OnlineWallets.ApiTests.Config
{
    public static class ScenarioContextKeys
    {
        public const string EndPoint = "EndPoint";
        public const string ResponseContent = "ResponseContent";
        public const string DepositRequest = "DepositRequest";
        public const string WithdrawalRequest = "WithdrawalRequest";
        public const string CurrentBalance = "CurrentBalance";
        public const string ErrorResponse = "ErrorResponse";
        public const string InitialBalance = "InitialBalance";
    }
}
