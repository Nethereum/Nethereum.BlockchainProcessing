using System.Threading.Tasks;
using Nethereum.BlockProcessing.ValueObjects;
using Nethereum.Web3;

namespace Nethereum.BlockchainProcessing.Processors.Transactions
{
    public class ContractCreatedCrawlerStep: CrawlerStep<TransactionWithReceipt, ContractCreationTransaction>
    {
        public bool RetrieveCode { get; set; } = false;
        public ContractCreatedCrawlerStep(IWeb3 web3) : base(web3)
        {
        }

        public override async Task<ContractCreationTransaction> GetStepDataAsync(TransactionWithReceipt transactionWithReceipt)
        {
            var contractAddress = transactionWithReceipt.TransactionReceipt.ContractAddress;
            bool? hasFailed = transactionWithReceipt.TransactionReceipt.HasErrors();
            string code = null;
            if (RetrieveCode && hasFailed != null)
            {
                code = await Web3.Eth.GetCode.SendRequestAsync(contractAddress).ConfigureAwait(false);
                hasFailed = HasFailedToCreateContract(code);
            }
            return new ContractCreationTransaction(transactionWithReceipt, code,
                hasFailed.Value);
        }

        protected virtual bool HasFailedToCreateContract(string code)
        {
            return (code == null) || (code == "0x");
        }
    }



}