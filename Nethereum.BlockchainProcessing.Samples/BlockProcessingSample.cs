using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.Processor;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Nethereum.BlockchainProcessing.Samples
{
    public class BlockProcessingSample
    {
        private readonly ITestOutputHelper output;

        public BlockProcessingSample(ITestOutputHelper output)
        {
            this.output = output;
        }

        /// <summary>
        /// Crawl the chain for a block range and injest the data
        /// </summary>
        [Fact]
        public async Task StartHere()
        {
            var blocks = new List<BlockWithTransactions>();
            var transactions = new List<TransactionReceiptVO>();
            var contractCreations = new List<ContractCreationVO>();
            var filterLogs = new List<FilterLogVO>();

            var web3 = new Web3.Web3(TestConfiguration.BlockchainUrls.Infura.Rinkeby);

            //create our processor
            var processor = web3.Processing.Blocks.CreateBlockProcessor(steps =>
            {
                // inject handler for each step
                steps.BlockStep.AddSynchronousProcessorHandler(b => blocks.Add(b));
                steps.TransactionReceiptStep.AddSynchronousProcessorHandler(tx => transactions.Add(tx));
                steps.ContractCreationStep.AddSynchronousProcessorHandler(cc => contractCreations.Add(cc));
                steps.FilterLogStep.AddSynchronousProcessorHandler(l => filterLogs.Add(l));
            });

            //if we need to stop the processor mid execution - call cancel on the token
            var cancellationToken = new CancellationToken();

            //crawl the required block range
            await processor.ExecuteAsync(
                toBlockNumber: new BigInteger(2830145),
                cancellationToken: cancellationToken,
                startAtBlockNumberIfNotProcessed: new BigInteger(2830144));

            Assert.Equal(2, blocks.Count);
            Assert.Equal(25, transactions.Count);
            Assert.Equal(5, contractCreations.Count);

            Log(transactions, contractCreations, filterLogs);
        }

        /// <summary>
        /// Demonstrates how to cancel processing
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Cancellation()
        {
            var blocks = new List<BlockWithTransactions>();

            var web3 = new Web3.Web3(TestConfiguration.BlockchainUrls.Infura.Rinkeby);

            //if we need to stop the processor mid execution - call cancel on the token
            var cancellationTokenSource = new CancellationTokenSource();

            //create our processor
            var processor = web3.Processing.Blocks.CreateBlockProcessor(steps =>
            {
                // inject handler
                // cancel after the first block is processsed
                steps.BlockStep.AddSynchronousProcessorHandler(b => { blocks.Add(b); cancellationTokenSource.Cancel(); });
            });


            //crawl the required block range
            await processor.ExecuteAsync(
                cancellationToken: cancellationTokenSource.Token, 2830144);

            Assert.Single(blocks);
        }

        /// <summary>
        /// Process transactions matching specific criteria
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task WithTransactionCriteria()
        {
            var transactions = new List<TransactionReceiptVO>();
            var filterLogs = new List<FilterLogVO>();

            var web3 = new Web3.Web3(TestConfiguration.BlockchainUrls.Infura.Rinkeby);

            //create our processor
            var processor = web3.Processing.Blocks.CreateBlockProcessor(steps => {
                steps.TransactionStep.SetMatchCriteria(t => t.Transaction.IsFrom("0x1cbff6551b8713296b0604705b1a3b76d238ae14"));
                steps.TransactionReceiptStep.AddSynchronousProcessorHandler(tx => transactions.Add(tx));
                steps.FilterLogStep.AddSynchronousProcessorHandler(l => filterLogs.Add(l));
            });

            //if we need to stop the processor mid execution - call cancel on the token
            var cancellationToken = new CancellationToken();
            //crawl the blocks
            await processor.ExecuteAsync(
                toBlockNumber: new BigInteger(2830145),
                cancellationToken: cancellationToken,
                startAtBlockNumberIfNotProcessed: new BigInteger(2830144));

            Assert.Equal(2, transactions.Count);
            Assert.Equal(4, filterLogs.Count);
        }

        /// <summary>
        /// Involving a specific contract
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ForASpecificContract()
        {
            var transactions = new List<TransactionReceiptVO>();
            var filterLogs = new List<FilterLogVO>();

            var web3 = new Web3.Web3(TestConfiguration.BlockchainUrls.Infura.Rinkeby);

            const string ContractAddress = "0x5534c67e69321278f5258f5bebd5a2078093ec19";

            //create our processor
            var processor = web3.Processing.Blocks.CreateBlockProcessor(steps => {
                //for performance we add criteria before we have the receipt to prevent unecessary data retrieval
                //we only want to retrieve receipts if the tx was sent to the contract
                steps.TransactionStep.SetMatchCriteria(t => t.Transaction.IsTo(ContractAddress));
                steps.TransactionReceiptStep.AddSynchronousProcessorHandler(tx => transactions.Add(tx));
                steps.FilterLogStep.AddSynchronousProcessorHandler(l => filterLogs.Add(l));
            });

            //if we need to stop the processor mid execution - call cancel on the token
            var cancellationToken = new CancellationToken();
            //crawl the blocks
            await processor.ExecuteAsync(
                toBlockNumber: new BigInteger(2830145),
                cancellationToken: cancellationToken,
                startAtBlockNumberIfNotProcessed: new BigInteger(2830144));

            Assert.Equal(2, transactions.Count);
            Assert.Equal(8, filterLogs.Count);
        }


        [Function("buyApprenticeChest")]
        public class BuyApprenticeFunction : FunctionMessage
        {
            [Parameter("uint256", "_region", 1)]
            public BigInteger Region { get; set; }
        }

        [Fact]
        public async Task ForASpecificFunction()
        {
            var transactions = new List<TransactionReceiptVO>();
            var filterLogs = new List<FilterLogVO>();

            var web3 = new Web3.Web3(TestConfiguration.BlockchainUrls.Infura.Rinkeby);

            //create our processor
            var processor = web3.Processing.Blocks.CreateBlockProcessor(steps => {
                
                //match the to address and function signature
                steps.TransactionStep.SetMatchCriteria(t => 
                    t.Transaction.IsTo("0xc03cdd393c89d169bd4877d58f0554f320f21037") && 
                    t.Transaction.IsTransactionForFunctionMessage<BuyApprenticeFunction>());

                steps.TransactionReceiptStep.AddSynchronousProcessorHandler(tx => transactions.Add(tx));
                steps.FilterLogStep.AddSynchronousProcessorHandler(l => filterLogs.Add(l));
            });

            //if we need to stop the processor mid execution - call cancel on the token
            var cancellationToken = new CancellationToken();
            //crawl the blocks
            await processor.ExecuteAsync(
                toBlockNumber: new BigInteger(3146684),
                cancellationToken: cancellationToken,
                startAtBlockNumberIfNotProcessed: new BigInteger(3146684));

            Assert.Single(transactions);
            Assert.Single(filterLogs);
        }

        private void Log(List<TransactionReceiptVO> transactions, List<ContractCreationVO> contractCreations, List<FilterLogVO> filterLogs)
        {
            output.WriteLine("Sent From");
            foreach (var fromAddressGrouping in transactions.GroupBy(t => t.Transaction.From).OrderByDescending(g => g.Count()))
            {
                var logs = filterLogs.Where(l => fromAddressGrouping.Any((a) => l.Transaction.TransactionHash == a.TransactionHash));

                output.WriteLine($"From: {fromAddressGrouping.Key}, Tx Count: {fromAddressGrouping.Count()}, Logs: {logs.Count()}");
            }

            output.WriteLine("Sent To");
            foreach (var toAddress in transactions
                .Where(t => !t.Transaction.IsToAnEmptyAddress())
                .GroupBy(t => t.Transaction.To)
                .OrderByDescending(g => g.Count()))
            {
                var logs = filterLogs.Where(l => toAddress.Any((a) => l.Transaction.TransactionHash == a.TransactionHash));

                output.WriteLine($"To: {toAddress.Key}, Tx Count: {toAddress.Count()}, Logs: {logs.Count()}");
            }

            output.WriteLine("Contracts Created");
            foreach (var contractCreated in contractCreations)
            {
                var tx = transactions.Count(t => t.Transaction.IsTo(contractCreated.ContractAddress));
                var logs = filterLogs.Count(l => transactions.Any(t => l.Transaction.TransactionHash == t.TransactionHash));

                output.WriteLine($"From: {contractCreated.ContractAddress}, Tx Count: {tx}, Logs: {logs}");
            }
        }
    }
}


