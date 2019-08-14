using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.BlockchainProcessing.Processor;
using Nethereum.RPC.Eth.DTOs;
using System.Collections.Generic;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public interface IEventSubscription: IProcessorHandler<FilterLog>
    {
        long Id {get;}
        long SubscriberId {get;}

        IEventSubscriptionStateDto State { get; }

        IEnumerable<IEventHandler> EventHandlers { get;}
    }
}
