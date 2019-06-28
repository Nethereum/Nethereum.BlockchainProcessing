using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainProcessing.Processors;
using System.Numerics;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Repositories
{

    public class StorageProcessingStrategy: ProcessingStrategy, IBlockchainProcessingStrategy
    {
        private readonly RepositoryHandlerContext _repositoryHandlerContext;

        public StorageProcessingStrategy(
            RepositoryHandlerContext repositoryHandlerContext,
            IBlockProgressRepository blockProgressRepository,
            IBlockProcessor blockProcessor):base(blockProcessor, blockProgressRepository)
        {
            _repositoryHandlerContext = repositoryHandlerContext;
        }

        public override async Task FillContractCacheAsync()
        {
            await _repositoryHandlerContext.ContractRepository.FillCache().ConfigureAwait(false);
        }
    }
}
