using Nethereum.BlockchainProcessing.Processor;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling.Handlers.Handlers
{
    public class StorageHandler: EventHandlerBase, IEventHandler
    {
        public StorageHandler(
            IEventSubscription subscription, 
            long id, 
            IProcessorHandler<FilterLog> logHandler) :base(subscription, id)
        {
            LogHandler = logHandler;
        }

        public IProcessorHandler<FilterLog> LogHandler { get; }

        public async Task<bool> HandleAsync(DecodedEvent decodedEvent)
        {
            await LogHandler.ExecuteAsync(decodedEvent.Log).ConfigureAwait(false);
            return true;
        }

    }
}
