using Microsoft.Extensions.Configuration;

namespace IconTestFramework.Core.Config
{
    public static class Configurator
    {
        public static IConfigurationRoot Config = new ConfigurationBuilder()
                                                    .SetBasePath(Directory.GetCurrentDirectory() + @"..\..\..\..\..\Betsson.OnlineWallets.ApiTests\Config")
                                                    .AddJsonFile("appsettings.json")
                                                    .Build();

        public static string BaseUrl => GetAppSettings("base_url");
        public static string DepositUrl => GetAppSettings("deposit_url");
        public static string BalanceUrl => GetAppSettings("balance_url");
        public static string WithdrawalUrl => GetAppSettings("withdrawal_url");

        private static string GetAppSettings(string name) => Config["AppSettings:" + name];
    }
}
