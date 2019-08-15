using Nethereum.BlockchainProcessing.Processor;
using Nethereum.BlockchainStore.AzureTables.Bootstrap;
using Nethereum.LogProcessing.Dynamic.Configuration;
using Nethereum.LogProcessing.Dynamic.Handling;
using Nethereum.RPC.Eth.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.LogProcessing.Dynamic.Db.Azure.Factories
{
    public class AzureTablesSubscriberRepositoryFactory : ISubscriberStorageFactory
    {
        Dictionary<string, AzureTablesRepositoryFactory> _cloudTableSetups = new Dictionary<string, AzureTablesRepositoryFactory>();
        public AzureTablesSubscriberRepositoryFactory( 
            string azureStorageConnectionString)
        {
            AzureStorageConnectionString = azureStorageConnectionString;
        }

        public string AzureStorageConnectionString { get; }

        public Task<IProcessorHandler<FilterLog>> GetLogRepositoryHandlerAsync(string tablePrefix)
        {
            var cloudTableSetup = GetCloudTableSetup(tablePrefix);
            var repo = cloudTableSetup.CreateTransactionLogRepository();
            IProcessorHandler<FilterLog> handler = new ProcessorHandler<FilterLog>(
                async (log) => await repo.UpsertAsync(new FilterLogVO(null, null, log)));
            return Task.FromResult(handler);
        }

        public Task<IProcessorHandler<FilterLog>> GetLogRepositoryHandlerAsync(ISubscriberStorageDto config) => 
            GetLogRepositoryHandlerAsync(config.Name);

        private AzureTablesRepositoryFactory GetCloudTableSetup(string tablePrefix)
        {
            if(!_cloudTableSetups.TryGetValue(tablePrefix, out AzureTablesRepositoryFactory setup))
            {
                setup = new AzureTablesRepositoryFactory(AzureStorageConnectionString, tablePrefix);
                _cloudTableSetups.Add(tablePrefix, setup);
            }
            return setup;
        }

        public async Task DeleteTablesAsync()
        {
            foreach(var prefix in _cloudTableSetups.Keys)
            {
                await _cloudTableSetups[prefix].GetTransactionsLogTable().DeleteIfExistsAsync();
            }
        }
    }
}
