using Nethereum.BlockchainProcessing.Queue.Azure.Processing.Logs;
using Nethereum.BlockchainStore.AzureTables.Bootstrap;
using Nethereum.BlockchainStore.Search;
using Nethereum.BlockchainStore.Search.Azure;
using Nethereum.Hex.HexTypes;
using Nethereum.LogProcessing.Dynamic.Configuration;
using Nethereum.LogProcessing.Dynamic.Db.Azure.Factories;
using Nethereum.LogProcessing.Dynamic.Handling.Handlers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.LogProcessing.Dynamic.IntegrationTests.Dynamic
{
    public class EventProcessingAsAService
    {
        const long PARTITION = 1;
        const ulong BLOCK_FROM = 4063361;
        const ulong BLOCK_TO = 4063370;
        const string AZURE_SEARCH_SERVICE_NAME = "blockchainsearch";

        [Fact]
        public async Task WebJobExample()
        {
            var config = TestConfiguration.LoadConfig();
            string azureStorageConnectionString = config["AzureStorageConnectionString"];
            string azureSearchKey = config["AzureSearchApiKey"];

            var configurationContext = EventProcessingConfigMock.Create(PARTITION, out IdGenerator idGenerator);
            IEventProcessingConfigurationRepository configurationRepository = configurationContext.CreateMockRepository(idGenerator);

            var web3 = new Web3.Web3(TestConfiguration.BlockchainUrls.Infura.Rinkeby);

            // search components
            var searchService = new AzureSearchService(serviceName: AZURE_SEARCH_SERVICE_NAME, searchApiKey: azureSearchKey);

            var subscriberSearchIndexFactory = new SubscriberSearchIndexFactory(async indexName => 
            { 
                if(await searchService.IndexExistsAsync(indexName) == false)
                {
                    //TODO: REPLACE THIS WITH Nethereum.BlockchainStore.Search.Azure.EventToGenericSearchDocMapper

                    await searchService.CreateIndexAsync(EventToGenericSearchDocMapper.CreateAzureIndexDefinition(indexName));
                }
                    
                return searchService.CreateIndexer<DecodedEvent, GenericSearchDocument>(
                    indexName, decodedEvent => EventToGenericSearchDocMapper.Map(decodedEvent, decodedEvent.State));
            });

            // queue components
            //AzureStorageQueueFactory
            var azureQueueFactory = new AzureStorageQueueFactory(azureStorageConnectionString);

            var subscriberQueueFactory = new SubscriberQueueFactory(
                queueName => azureQueueFactory.GetOrCreateQueueAsync(queueName));

            // subscriber repository
            var repositoryFactory = new AzureTablesSubscriberRepositoryFactory(azureStorageConnectionString);

            // load subscribers and event subscriptions
            var eventSubscriptionFactory = new EventSubscriptionFactory(
                web3, configurationRepository, subscriberQueueFactory, subscriberSearchIndexFactory, repositoryFactory);

            var eventSubscriptions = await eventSubscriptionFactory.LoadAsync(PARTITION);

            // progress repo (dictates which block ranges to process next)
            // maintain separate progress per partition via a prefix
            var storageCloudSetup = new AzureTablesRepositoryFactory(azureStorageConnectionString, prefix: $"Partition{PARTITION}");
            var blockProgressRepo = storageCloudSetup.CreateBlockProgressRepository();

            var logProcessor = web3.Processing.Logs.CreateProcessor(
                logProcessors: eventSubscriptions, blockProgressRepository: blockProgressRepo);

            // execute
            try
            {
                var ctx = new System.Threading.CancellationTokenSource();
                await logProcessor.ExecuteAsync(BLOCK_TO, ctx.Token, BLOCK_FROM);
            }
            finally
            {
                await ClearDown(configurationContext, storageCloudSetup, searchService, azureQueueFactory, repositoryFactory);
            }

            // save event subscription state
            await configurationRepository.EventSubscriptionStates.UpsertAsync(eventSubscriptions.Select(s => s.State));
            
            // assertions

            var subscriptionState1 = configurationContext.GetEventSubscriptionState(eventSubscriptionId: 1); // interested in transfers with contract queries and aggregations
            var subscriptionState2 = configurationContext.GetEventSubscriptionState(eventSubscriptionId: 2); // interested in transfers with simple aggregation
            var subscriptionState3 = configurationContext.GetEventSubscriptionState(eventSubscriptionId: 3); // interested in any event for a specific address

            Assert.Equal("4009000000002040652615", subscriptionState1.Values["RunningTotalForTransferValue"].ToString());
            Assert.Equal((uint)19, subscriptionState2.Values["CurrentTransferCount"]);

            var txForSpecificAddress = (List<string>)subscriptionState3.Values["AllTransactionHashes"];
            Assert.Equal("0x362bcbc78a5cc6156e8d24d95bee6b8f53d7821083940434d2191feba477ae0e", txForSpecificAddress[0]);
            Assert.Equal("0xe63e9422dedf84d0ce13f9f75ebfd86333ce917b2572925fbdd51b51caf89b77", txForSpecificAddress[1]);

            var blockNumbersForSpecificAddress = (List<HexBigInteger>)subscriptionState3.Values["AllBlockNumbers"];
            Assert.Equal(4063362, blockNumbersForSpecificAddress[0].Value);
            Assert.Equal(4063362, blockNumbersForSpecificAddress[1].Value);

        }

        private async Task ClearDown(
            EventProcessingConfigContext repo,
            AzureTablesRepositoryFactory cloudTableSetup, 
            IAzureSearchService searchService,
            AzureStorageQueueFactory subscriberQueueFactory,
            AzureTablesSubscriberRepositoryFactory azureTablesSubscriberRepositoryFactory)
        {
            foreach(var index in repo.SubscriberSearchIndexes) 
            { 
                await searchService.DeleteIndexAsync(index.Name);
            }

            foreach (var queue in repo.SubscriberSearchIndexes)
            {
                await subscriberQueueFactory.DeleteQueueAsync(queue.Name);
            }

            await cloudTableSetup.GetCountersTable().DeleteIfExistsAsync();

            await azureTablesSubscriberRepositoryFactory.DeleteTablesAsync();
        }

    }
}