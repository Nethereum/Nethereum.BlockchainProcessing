using Nethereum.BlockchainProcessing.Processing.Logs.Configuration;
using Nethereum.BlockchainProcessing.Processor;
using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public interface ISubscriberStorageFactory
    {
        Task<IProcessorHandler<FilterLog>> GetLogRepositoryHandlerAsync(ISubscriberStorageDto dto);
    }
}
