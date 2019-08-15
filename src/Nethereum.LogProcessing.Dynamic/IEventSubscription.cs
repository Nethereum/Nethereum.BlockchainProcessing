

using Nethereum.BlockchainProcessing.Processor;
using Nethereum.LogProcessing.Dynamic.Configuration;
using Nethereum.LogProcessing.Dynamic.Handling;
using Nethereum.RPC.Eth.DTOs;
using System.Collections.Generic;

namespace Nethereum.LogProcessing.Dynamic
{
    public interface IEventSubscription: IProcessorHandler<FilterLog>
    {
        long Id {get;}
        long SubscriberId {get;}

        IEventSubscriptionStateDto State { get; }

        IEnumerable<IEventHandler> EventHandlers { get;}
    }
}
