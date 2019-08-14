using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainProcessing.Queue;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public interface ISubscriberQueueFactory
    {
        Task<IQueue> GetSubscriberQueueAsync(ISubscriberQueueDto dto);
    }
}
