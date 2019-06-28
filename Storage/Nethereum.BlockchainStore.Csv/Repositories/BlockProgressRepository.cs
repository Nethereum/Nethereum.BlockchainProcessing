using CsvHelper.Configuration;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainStore.Entities;
using System.Numerics;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Csv.Repositories
{

    public class BlockProgressRepository : CsvRepositoryBase<BlockProgress>, IBlockProgressRepository
    {
        public BlockProgressRepository(string csvFolderpath) : base(csvFolderpath, "BlockProgress")
        {
        }

        public async Task<BigInteger?> GetLastBlockNumberProcessedAsync()
        {
            BigInteger? maxBlock = null;

            await EnumerateAsync(e =>
            {
                var blockNumber = BigInteger.Parse(e.LastBlockProcessed);

                if (maxBlock == null || maxBlock.Value < blockNumber)
                {
                    maxBlock = blockNumber;
                }
            });

            return maxBlock;
        }

        public async Task UpsertProgressAsync(BigInteger blockNumber)
        {
            var blockProgress = new BlockProgress(){LastBlockProcessed = blockNumber.ToString() };
            blockProgress.UpdateRowDates();
            await Write(blockProgress).ConfigureAwait(false);
        }

        protected override ClassMap<BlockProgress> CreateClassMap()
        {
            return BlockProgressMap.Instance;
        }
    }

    public class BlockProgressMap : ClassMap<BlockProgress>
    {
        public static BlockProgressMap Instance = new BlockProgressMap();

        public BlockProgressMap()
        {
            AutoMap();
        }
    }
}
