using System.Threading.Tasks;
using Nethereum.BlockProcessing.ValueObjects;
using Nethereum.Web3;

namespace Nethereum.BlockchainProcessing.Processors.Transactions
{
    public class TransactionWithBlockCrawlerStep : CrawlerStep<TransactionWithBlock, TransactionWithBlock>
    {
        public TransactionWithBlockCrawlerStep(IWeb3 web3) : base(web3)
        {
        }

        public override Task<TransactionWithBlock> GetStepDataAsync(TransactionWithBlock parentStep)
        {
            return Task.FromResult(parentStep);
        }
    }
}