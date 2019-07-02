using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainProcessing.Processors;
using System.Numerics;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Repositories
{

    public class StorageBlockchainProcessingStrategy: BlockchainProcessingStrategy, IBlockchainProcessingStrategy
    {
        private readonly RepositoryHandlerContext _repositoryHandlerContext;

        public StorageProcessingStrategy(
            RepositoryHandlerContext repositoryHandlerContext,
            IBlockProgressRepository blockProgressRepository,
            IBlockCrawler blockCrawler):base(blockCrawler, blockProgressRepository)
        {
            _repositoryHandlerContext = repositoryHandlerContext;
        }

        public override async Task FillContractCacheAsync()
        {
            await _repositoryHandlerContext.ContractRepository.FillCache().ConfigureAwait(false);
        }
    }
}
