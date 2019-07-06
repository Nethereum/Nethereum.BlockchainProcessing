using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.Common.Processing;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainStore.Repositories.Handlers
{
    public class BlockStepStorageHandler : IProcessorHandler<Block>
    {
        private readonly IBlockRepository _blockRepository;

        public BlockStepStorageHandler(IBlockRepository blockRepository)
        {
            _blockRepository = blockRepository;
        }

        public Task ExecuteAsync(Block block)
        {
            return _blockRepository.UpsertBlockAsync(block);
        }
    }
}
