using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.Processor;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Nethereum.BlockchainProcessing.Samples
{
    public class LogProcessingSample
    {

        [Event("Transfer")]
        public class TransferEvent: IEventDTO
        {
            [Parameter("address", "_from", 1, true)]
            public string From { get; set; }

            [Parameter("address", "_to", 2, true)]
            public string To { get; set; }

            [Parameter("uint256", "_value", 3, false)]
            public BigInteger Value { get; set; }
        }

        [Event("Transfer")]
        public class Erc721TransferEvent
        {
            [Parameter("address", "_from", 1, true)]
            public string From { get; set; }

            [Parameter("address", "_to", 2, true)]
            public string To { get; set; }

            [Parameter("uint256", "_value", 3, true)]
            public BigInteger Value { get; set; }
        }

        [Fact]
        public async Task OneContractOneEvent()
        {
            var transferEventLogs = new List<EventLog<TransferEvent>>();

            var web3 = new Web3.Web3(TestConfiguration.BlockchainUrls.Infura.Rinkeby);

            //create our processor to retrieve transfers
            //restrict the processor to Transfers for a specific contract address
            var processor = web3.Processing.Logs.CreateProcessorForContract<TransferEvent>("0x109424946d5aa4425b2dc1934031d634cdad3f90", tfr => transferEventLogs.Add(tfr));

            //if we need to stop the processor mid execution - call cancel on the token
            var cancellationToken = new CancellationToken();

            //crawl the required block range
            await processor.ExecuteAsync(
                toBlockNumber: new BigInteger(3146690),
                cancellationToken: cancellationToken,
                startAtBlockNumberIfNotProcessed: new BigInteger(3146684));

            Assert.Single(transferEventLogs);
        }

        [Fact]
        public async Task OneContractOneEventWithCriteria()
        {
            var transferEventLogs = new List<EventLog<TransferEvent>>();

            var web3 = new Web3.Web3(TestConfiguration.BlockchainUrls.Infura.Rinkeby);

            //create our processor to retrieve transfers
            //restrict the processor to Transfers for a specific contract address
            var processor = web3.Processing.Logs.CreateProcessorForContract<TransferEvent>(
                "0x109424946d5aa4425b2dc1934031d634cdad3f90", 
                action: tfr => transferEventLogs.Add(tfr),
                criteria: tfr => tfr.Event.Value > 0);

            //if we need to stop the processor mid execution - call cancel on the token
            var cancellationToken = new CancellationToken();

            //crawl the required block range
            await processor.ExecuteAsync(
                toBlockNumber: new BigInteger(3146690),
                cancellationToken: cancellationToken,
                startAtBlockNumberIfNotProcessed: new BigInteger(3146684));

            Assert.Single(transferEventLogs);
        }

        [Fact]
        public async Task OneContractOneEventAsync()
        {
            var transferEventLogs = new List<EventLog<TransferEvent>>();

            var web3 = new Web3.Web3(TestConfiguration.BlockchainUrls.Infura.Rinkeby);

            Task StoreLogAsync(EventLog<TransferEvent> eventLog)
            {
                transferEventLogs.Add(eventLog);
                return Task.CompletedTask;
            }

            //create our processor to retrieve transfers
            //restrict the processor to Transfers for a specific contract address
            var processor = web3.Processing.Logs.CreateProcessorForContract<TransferEvent>(
                "0x109424946d5aa4425b2dc1934031d634cdad3f90", StoreLogAsync);

            //if we need to stop the processor mid execution - call cancel on the token
            var cancellationToken = new CancellationToken();

            //crawl the required block range
            await processor.ExecuteAsync(
                toBlockNumber: new BigInteger(3146690),
                cancellationToken: cancellationToken,
                startAtBlockNumberIfNotProcessed: new BigInteger(3146684));

            Assert.Single(transferEventLogs);
        }

        [Fact]
        public async Task AnyContractManyEventAsync()
        {
            var erc20transferEventLogs = new List<EventLog<TransferEvent>>();
            var erc721TransferEventLogs = new List<EventLog<Erc721TransferEvent>>();

            var web3 = new Web3.Web3(TestConfiguration.BlockchainUrls.Infura.Rinkeby);

            var erc20TransferHandler = new EventLogProcessorHandler<TransferEvent>(
                eventLog => erc20transferEventLogs.Add(eventLog));

            var erc721TransferHandler = new EventLogProcessorHandler<Erc721TransferEvent>(
                eventLog => erc721TransferEventLogs.Add(eventLog)); 

            var processingHandlers = new ProcessorHandler<FilterLog>[] {
                erc20TransferHandler, erc721TransferHandler};

            //create our processor to retrieve transfers
            //restrict the processor to Transfers for a specific contract address
            var processor = web3.Processing.Logs.CreateProcessor(processingHandlers);

            //if we need to stop the processor mid execution - call cancel on the token
            var cancellationToken = new CancellationToken();

            //crawl the required block range
            await processor.ExecuteAsync(
                toBlockNumber: new BigInteger(3146690),
                cancellationToken: cancellationToken,
                startAtBlockNumberIfNotProcessed: new BigInteger(3146684));

            Assert.Equal(13, erc20transferEventLogs.Count);
            Assert.Equal(3, erc721TransferEventLogs.Count);
        }

        [Fact]
        public async Task AnyContractOneEvent()
        {
            var transferEventLogs = new List<EventLog<TransferEvent>>();

            var web3 = new Web3.Web3(TestConfiguration.BlockchainUrls.Infura.Rinkeby);

            //create our processor to retrieve transfers
            //restrict the processor to Transfers
            var processor = web3.Processing.Logs.CreateProcessor<TransferEvent>(
                tfr => transferEventLogs.Add(tfr));

            //if we need to stop the processor mid execution - call cancel on the token
            var cancellationToken = new CancellationToken();

            //crawl the required block range
            await processor.ExecuteAsync(
                toBlockNumber: new BigInteger(3146690),
                cancellationToken: cancellationToken,
                startAtBlockNumberIfNotProcessed: new BigInteger(3146684));

            Assert.Equal(13, transferEventLogs.Count);
        }

        [Fact]
        public async Task ManyContractsOneEvent()
        {
            var transferEventLogs = new List<EventLog<TransferEvent>>();

            var web3 = new Web3.Web3(TestConfiguration.BlockchainUrls.Infura.Rinkeby);

            var contractAddresses = new [] { "0x109424946d5aa4425b2dc1934031d634cdad3f90", "0x16c45b25c4817bdedfce770f795790795c9505a6" };

            //create our processor to retrieve transfers
            //restrict the processor to Transfers for a specific contract address
            var processor = web3.Processing.Logs.CreateProcessorForContracts<TransferEvent>(contractAddresses, tfr => transferEventLogs.Add(tfr));

            //if we need to stop the processor mid execution - call cancel on the token
            var cancellationToken = new CancellationToken();

            //crawl the required block range
            await processor.ExecuteAsync(
                toBlockNumber: new BigInteger(3146690),
                cancellationToken: cancellationToken,
                startAtBlockNumberIfNotProcessed: new BigInteger(3146684));

            Assert.Equal(5, transferEventLogs.Count);
        }

        [Fact]
        public async Task AnyContractAnyLog()
        {
            var logs = new List<FilterLog>();

            var web3 = new Web3.Web3(TestConfiguration.BlockchainUrls.Infura.Rinkeby);

            //create our processor to retrieve transfers
            var processor = web3.Processing.Logs.CreateProcessor(log => logs.Add(log));

            //if we need to stop the processor mid execution - call cancel on the token
            var cancellationToken = new CancellationToken();

            //crawl the required block range
            await processor.ExecuteAsync(
                toBlockNumber: new BigInteger(3146690),
                cancellationToken: cancellationToken,
                startAtBlockNumberIfNotProcessed: new BigInteger(3146684));

            Assert.Equal(65, logs.Count);
        }


        [Fact]
        public async Task OneContractAnyLog()
        {
            var logs = new List<FilterLog>();

            var web3 = new Web3.Web3(TestConfiguration.BlockchainUrls.Infura.Rinkeby);

            //create our processor to retrieve transfers
            var processor = web3.Processing.Logs.CreateProcessorForContract(
                "0x109424946d5aa4425b2dc1934031d634cdad3f90", log => logs.Add(log));

            //if we need to stop the processor mid execution - call cancel on the token
            var cancellationToken = new CancellationToken();

            //crawl the required block range
            await processor.ExecuteAsync(
                toBlockNumber: new BigInteger(3146690),
                cancellationToken: cancellationToken,
                startAtBlockNumberIfNotProcessed: new BigInteger(3146684));

            Assert.Equal(4, logs.Count);
        }

        [Fact]
        public async Task AnyContractAnyLogWithCriteria()
        {
            var logs = new List<FilterLog>();

            var web3 = new Web3.Web3(TestConfiguration.BlockchainUrls.Infura.Rinkeby);

            var processor = web3.Processing.Logs.CreateProcessor(
                action: log => logs.Add(log), 
                criteria: log => log.Removed == false);

            //if we need to stop the processor mid execution - call cancel on the token
            var cancellationToken = new CancellationToken();

            //crawl the required block range
            await processor.ExecuteAsync(
                toBlockNumber: new BigInteger(3146690),
                cancellationToken: cancellationToken,
                startAtBlockNumberIfNotProcessed: new BigInteger(3146684));

            Assert.Equal(65, logs.Count);
        }

    }
}


