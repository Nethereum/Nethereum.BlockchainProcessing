using System.Numerics;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processors
{
    public interface IBlockCrawler
    {
        bool ProcessTransactionsInParallel { get;set; }
        Task ProcessBlockAsync(BigInteger blockNumber);
        Task<BigInteger> GetMaxBlockNumberAsync();
    }
}