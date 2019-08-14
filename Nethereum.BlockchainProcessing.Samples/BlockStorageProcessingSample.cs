using Nethereum.BlockchainProcessing.BlockStorage.Repositories;
using Nethereum.RPC.Eth.DTOs;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Nethereum.BlockchainProcessing.Samples
{
    public class BlockStorageProcessingSample
    {
        private readonly ITestOutputHelper output;

        public BlockStorageProcessingSample(ITestOutputHelper output)
        {
            this.output = output;
        }

        /// <summary>
        /// Crawl the chain for a block range and injest the data
        /// </summary>
        [Fact]
        public async Task StartHere()
        {
            var web3 = new Web3.Web3(TestConfiguration.BlockchainUrls.Infura.Rinkeby);

            //create an in-memory context and repository factory 
            var context = new InMemoryBlockchainStorageRepositoryContext();
            var repoFactory = new InMemoryBlockchainStoreRepositoryFactory(context);

            //create our processor
            var processor = web3.Processing.Blocks.CreateBlockStorageProcessor(repoFactory);

            //if we need to stop the processor mid execution - call cancel on the token
            var cancellationToken = new CancellationToken();

            //crawl the required block range
            await processor.ExecuteAsync(
                toBlockNumber: new BigInteger(2830145),
                cancellationToken: cancellationToken,
                startAtBlockNumberIfNotProcessed: new BigInteger(2830144));

            Assert.Equal(2, context.Blocks.Count);
            Assert.Equal(25, context.Transactions.Count);
            Assert.Equal(5, context.Contracts.Count);
            Assert.Equal(55, context.AddressTransactions.Count);
            Assert.Equal(28, context.TransactionLogs.Count);
        }

        [Fact]
        public async Task WithCriteria()
        {
            var web3 = new Web3.Web3(TestConfiguration.BlockchainUrls.Infura.Rinkeby);

            //create an in-memory context and repository factory 
            var context = new InMemoryBlockchainStorageRepositoryContext();
            var repoFactory = new InMemoryBlockchainStoreRepositoryFactory(context);

            //create our processor - we're only interested in tx from a specific address
            var processor = web3.Processing.Blocks.CreateBlockStorageProcessor(repoFactory, configureSteps: steps => {
                steps.TransactionStep.SetMatchCriteria(t => t.Transaction.IsFrom("0x1cbff6551b8713296b0604705b1a3b76d238ae14"));
            });

            //if we need to stop the processor mid execution - call cancel on the token
            var cancellationToken = new CancellationToken();

            //crawl the required block range
            await processor.ExecuteAsync(
                toBlockNumber: new BigInteger(2830145),
                cancellationToken: cancellationToken,
                startAtBlockNumberIfNotProcessed: new BigInteger(2830144));

            Assert.Equal(2, context.Blocks.Count);
            Assert.Equal(2, context.Transactions.Count);
            Assert.Equal(4, context.TransactionLogs.Count);
        }

    }
}


