using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.Common.Processing;
using Nethereum.BlockchainProcessing.Processors.Transactions;
using Nethereum.BlockProcessing.ValueObjects;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace Nethereum.BlockchainProcessing.Processors
{
    public interface IBlockCrawlOrchestrator
    {
        Task CrawlBlock(BigInteger bigInteger);
    }
    public class BlockCrawlOrchestrator: IBlockCrawlOrchestrator
    {
        protected IWeb3 Web3 { get; set; }
        public IEnumerable<BlockchainProcessorExecutionSteps> ExecutionStepsCollection { get; }
        protected BlockCrawlerStep BlockCrawlerStep { get; }
        protected TransactionWithBlockCrawlerStep TransactionWithBlockCrawlerStep { get; }
        protected TransactionWithReceiptCrawlerStep TransactionWithReceiptCrawlerStep { get; }

        public BlockCrawlOrchestrator(IWeb3 web3, IEnumerable<BlockchainProcessorExecutionSteps> executionStepsCollection)
        {
            this.ExecutionStepsCollection = executionStepsCollection;
            Web3 = web3;
            BlockCrawlerStep = new BlockCrawlerStep(web3);
            TransactionWithBlockCrawlerStep = new TransactionWithBlockCrawlerStep(web3);
            TransactionWithReceiptCrawlerStep = new TransactionWithReceiptCrawlerStep(web3);
        }

        public virtual async Task CrawlBlock(BigInteger blockNumber)
        {
            var blockCrawlerStepCompleted = await BlockCrawlerStep.ExecuteStepAsync(blockNumber, ExecutionStepsCollection);
            await CrawlTransactions(blockCrawlerStepCompleted);

        }
        protected virtual async Task CrawlTransactions(CrawlerStepCompleted<BlockWithTransactions> completedStep)
        {
            if (completedStep != null)
            {
                foreach (var txn in completedStep.StepData.Transactions)
                {
                    await CrawlTransaction(completedStep, txn);
                }
            }
        }
        protected virtual async Task CrawlTransaction(CrawlerStepCompleted<BlockWithTransactions> completedStep, Transaction txn)
        {
            var currentStepCompleted = await TransactionWithBlockCrawlerStep.ExecuteStepAsync(
                new TransactionWithBlock(txn, completedStep.StepData), completedStep.ExecutedStepsCollection);
            await CrawlTransactionReceipt(currentStepCompleted);
        }

        protected virtual async Task CrawlTransactionReceipt(CrawlerStepCompleted<TransactionWithBlock> currentStepCompleted)
        {
            await TransactionWithReceiptCrawlerStep.ExecuteStepAsync(currentStepCompleted.StepData,
                currentStepCompleted.ExecutedStepsCollection);
        }
    }
}