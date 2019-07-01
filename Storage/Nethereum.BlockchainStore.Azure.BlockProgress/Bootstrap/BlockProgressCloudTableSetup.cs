using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainStore.AzureTables.Repositories;
using Nethereum.BlockchainStore.Repositories;

namespace Nethereum.BlockchainStore.AzureTables.Bootstrap
{

    public class BlockProgressCloudTableSetup: CloudTableSetupBase
    {
        public BlockProgressCloudTableSetup(string connectionString, string prefix):base(connectionString, prefix){ }

        public CloudTable GetCountersTable()
        {
            return GetPrefixedTable("Counters");
        }

        public IBlockProgressRepository CreateBlockProgressRepository() => new BlockProgressRepository(GetCountersTable());

        public async Task DeleteAllTables()
        {
            var options = new TableRequestOptions { };
            var operationContext = new OperationContext() { };
            await GetCountersTable().DeleteIfExistsAsync(options, operationContext);
        }
    }
}