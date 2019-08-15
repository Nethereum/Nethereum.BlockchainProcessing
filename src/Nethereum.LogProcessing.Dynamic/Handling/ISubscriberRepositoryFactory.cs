
using Nethereum.BlockchainProcessing.Processor;
using Nethereum.LogProcessing.Dynamic.Configuration;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;

namespace Nethereum.LogProcessing.Dynamic.Handling
{
    public interface ISubscriberStorageFactory
    {
        Task<IProcessorHandler<FilterLog>> GetLogRepositoryHandlerAsync(ISubscriberStorageDto dto);
    }
}
