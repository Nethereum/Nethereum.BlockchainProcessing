using Nethereum.BlockchainStore.Search;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling.Handlers
{
    public class SubscriberSearchIndex : ISubscriberSearchIndex
    {
        public SubscriberSearchIndex(string indexName, IIndexer<DecodedEvent> indexer)
        {
            Name = indexName;
            Indexer = indexer;
        }

        public string Name { get; }

        public IIndexer<DecodedEvent> Indexer { get; }

        public async Task IndexAsync(DecodedEvent decodedEvent)
        {
            await Indexer.IndexAsync(decodedEvent).ConfigureAwait(false);
        }
    }


}