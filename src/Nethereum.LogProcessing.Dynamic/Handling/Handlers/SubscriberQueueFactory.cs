
using Nethereum.BlockchainProcessing.Queue;
using Nethereum.LogProcessing.Dynamic.Configuration;
using System;
using System.Threading.Tasks;

namespace Nethereum.LogProcessing.Dynamic.Handling.Handlers
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