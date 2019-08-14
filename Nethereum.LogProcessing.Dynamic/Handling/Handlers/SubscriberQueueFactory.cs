using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainProcessing.Queue;
using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling.Handlers
{
    public class SubscriberQueueFactory: ISubscriberQueueFactory
    {
        public SubscriberQueueFactory(Func<string, Task<IQueue>> createQueueFunc)
        {
            CreateQueue = createQueueFunc;
        }

        public Func<string, Task<IQueue>> CreateQueue { get; }

        public Task<IQueue> GetSubscriberQueueAsync(ISubscriberQueueDto dto) => CreateQueue(dto.Name);
    }
}