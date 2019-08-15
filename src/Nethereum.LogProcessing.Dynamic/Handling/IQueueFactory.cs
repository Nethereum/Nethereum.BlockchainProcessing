
using Nethereum.BlockchainProcessing.Queue;
using Nethereum.LogProcessing.Dynamic.Configuration;
using System.Threading.Tasks;

namespace Nethereum.LogProcessing.Dynamic.Handling
{
    public interface ISubscriberQueueFactory
    {
        Task<IQueue> GetSubscriberQueueAsync(ISubscriberQueueDto dto);
    }
}
