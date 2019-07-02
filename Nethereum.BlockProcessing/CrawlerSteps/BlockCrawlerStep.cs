using System.Numerics;
using System.Threading.Tasks;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace Nethereum.BlockchainProcessing.Processors
{
    public class BlockCrawlerStep : CrawlerStep<BigInteger, BlockWithTransactions>
    {
        public BlockCrawlerStep(IWeb3 web3) : base(web3)
        {

        }
        public override Task<BlockWithTransactions> GetStepDataAsync(BigInteger blockNumber)
        {
            return Web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(blockNumber.ToHexBigInteger());
        }
    }
}