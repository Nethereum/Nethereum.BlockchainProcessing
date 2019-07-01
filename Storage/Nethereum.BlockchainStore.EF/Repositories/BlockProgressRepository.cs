using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainStore.Entities;
using System.Data.Entity;
using System.Numerics;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.EF.Repositories
{
    public class BlockProgressRepository : RepositoryBase, IBlockProgressRepository
    {
        public BlockProgressRepository(IBlockchainDbContextFactory contextFactory) : base(contextFactory)
        {
        }

        public async Task<BigInteger?> GetLastBlockNumberProcessedAsync()
        {
            using (var context = _contextFactory.CreateContext())
            {
                var max = await context.BlockProgress.MaxAsync(b => b.LastBlockProcessed).ConfigureAwait(false);
                return string.IsNullOrEmpty(max) ? (BigInteger?)null : BigInteger.Parse(max);
            }
        }

        public async Task UpsertProgressAsync(BigInteger blockNumber)
        {
            var blockRange = new BlockProgress { LastBlockProcessed = blockNumber.ToString()};

            using (var context = _contextFactory.CreateContext())
            {
                blockRange.UpdateRowDates();

                context.BlockProgress.Add(blockRange);

                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}
