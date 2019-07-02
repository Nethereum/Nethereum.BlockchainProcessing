using System.Threading.Tasks;
using Nethereum.BlockProcessing.ValueObjects;
using Nethereum.Web3;

namespace Nethereum.BlockchainProcessing.Processors.Transactions
{
    public class TransactionWithReceiptCrawlerStep : CrawlerStep<TransactionWithBlock, TransactionWithReceipt>
    {
        public TransactionWithReceiptCrawlerStep(IWeb3 web3) : base(web3)
        {
        }

        public override async Task<TransactionWithReceipt> GetStepDataAsync(TransactionWithBlock transaction)
        {
            var receipt = await Web3.Eth.Transactions
                .GetTransactionReceipt.SendRequestAsync(transaction.Transaction.TransactionHash)
                .ConfigureAwait(false);
            return new TransactionWithReceipt(transaction.Block, transaction.Transaction, receipt, receipt.HasErrors()?? false);
        }
    }
}