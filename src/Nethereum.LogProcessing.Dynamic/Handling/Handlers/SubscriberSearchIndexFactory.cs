
using Nethereum.BlockchainStore.Search;
using Nethereum.LogProcessing.Dynamic.Configuration;
using System;
using System.Threading.Tasks;

namespace Nethereum.LogProcessing.Dynamic.Handling.Handlers
{
    public class SubscriberSearchIndexFactory : ISubscriberSearchIndexFactory
    {
        public SubscriberSearchIndexFactory(
            Func<string, Task<IIndexer<DecodedEvent>>> createIndexer)
        {
            CreateIndexer = createIndexer;
        }

        public Func<string, Task> CreateIfNotExists { get; }
        public Func<string, Task<IIndexer<DecodedEvent>>> CreateIndexer { get; }
        public async Task<ISubscriberSearchIndex> GetSubscriberSearchIndexAsync(ISubscriberSearchIndexDto config)
        {
            var indexer = await CreateIndexer(config.Name).ConfigureAwait(false);
            return new SubscriberSearchIndex(config.Name, indexer);
        }
    }


}