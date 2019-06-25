using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nethereum.BlockchainStore.EF;
using Nethereum.BlockchainStore.EF.Hana;
using Nethereum.BlockchainStore.Processing;
using Nethereum.BlockchainProcessing.Processing;
using Microsoft.Configuration.Utils;
using Microsoft.Logging.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Nethereum.BlockchainStore.EF.Hana.Console
{
    class Program
    {
        static int Main(string[] args)
        {
            var log = ApplicationLogging.CreateConsoleLogger<Program>().ToILog();

            var appConfig = ConfigurationUtils
                .Build(args, userSecretsId: "PurchaseOrderEthereumToSap");

            var configuration = BlockchainSourceConfigurationFactory.Get(appConfig);
            var hanaSchema = GetHanaSchema(appConfig);
            var hanaDbContextFactory = new HanaBlockchainDbContextFactory("BlockchainDbContext_hana", hanaSchema);
            var repositoryFactory = new BlockchainStoreRepositoryFactory(hanaDbContextFactory);
            return StorageProcessorConsole.Execute(repositoryFactory, configuration, log: log).Result;
        }

        private static string GetHanaSchema(IConfigurationRoot config)
        {
            string hanaSchema = config["HanaSchema"] ?? "DEMO";
            return hanaSchema;
        }
    }
}
