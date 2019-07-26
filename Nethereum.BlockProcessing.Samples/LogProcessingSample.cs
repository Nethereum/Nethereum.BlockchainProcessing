using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.Processor;
using Nethereum.BlockchainProcessing.ProgressRepositories;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Nethereum.BlockProcessing.Samples
{
    public class LogProcessingSample
    {
        private readonly ITestOutputHelper output;

        public LogProcessingSample(ITestOutputHelper output)
        {
            this.output = output;
        }

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
            var processor = web3.Processing.Logs.CreateProcessorForContract<TransferEvent>("0x109424946d5aa4425b2dc1934031d634cdad3f90", StoreLogAsync);

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

            var erc20TransferHandler = new EventLogProcessorHandler<TransferEvent>(eventLog => erc20transferEventLogs.Add(eventLog));
            var erc721TransferHandler = new EventLogProcessorHandler<Erc721TransferEvent>(eventLog => erc721TransferEventLogs.Add(eventLog)); 
            var processingHandlers = new ProcessorHandler<FilterLog>[] {erc20TransferHandler, erc721TransferHandler};

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
            var processor = web3.Processing.Logs.CreateProcessor<TransferEvent>(tfr => transferEventLogs.Add(tfr));

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
            var processor = web3.Processing.Logs.CreateProcessorForContract("0x109424946d5aa4425b2dc1934031d634cdad3f90", log => logs.Add(log));

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
        public async Task AnyContractAnyLogMatchingCriteria()
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

        [Fact]
        public async Task ProcessingAnEventForThousandsOfContracts()
        {
            // for the test - these addresses are coming from a file
            // obviously you can replace this with your own implementation
            // 7590 contract addresses expected
            HashSet<string> contractAddresses = LoadContractAddresses();

            // instantiate our web3 object
            var web3 = new Web3.Web3(TestConfiguration.BlockchainUrls.Infura.Rinkeby);

            // our contract address criteria
            // we're only interested in transfers on our contracts
            var criteria = new Func<EventLog<TransferEvent>, Task<bool>>(transferEventLog =>
            {
                return Task.FromResult(contractAddresses.Contains(transferEventLog.Log.Address));
            });

            // somewhere to store the balance of each account involved in a transfer
            // there's no opening balance implementation here - so some balances will end up negative
            var balances = new Dictionary<string, BigInteger>();

            // the action to handle each relevant transfer event
            // this is a trivial implementation (in memory balance management)
            // but your own can action can include much more complex logic
            // this may involve persistence, lookups and Async calls etc
            var action = new Func<EventLog<TransferEvent>, Task>(transferEventLog =>
            {
                var from = transferEventLog.Event.From;
                var to = transferEventLog.Event.To;

                if (!balances.ContainsKey(from)) balances.Add(from, 0);
                if (!balances.ContainsKey(to)) balances.Add(to, 0);

                balances[from] = balances[from] - transferEventLog.Event.Value;
                balances[to] = balances[to] + transferEventLog.Event.Value;

                return Task.CompletedTask;
            });

            //we're using an in memory progress repo which tracks the blocks processed
            //this can be used to run the processor continually and pick up where it left off on a restart
            //for real use - replace this with your own persistent implementation of IBlockProgressRepository
            var blockProgressRepository = new InMemoryBlockchainProgressRepository();

            // as we're handling thousands of contracts -
            // we're not defaulting to the more obvious CreateProcessorForContracts method
            // that would result in creating a filter containing all of the addresses
            // that filter would be sent to the node on a GetLogs RPC request
            // that large filter may cause issues depending on the node/client
            // as this is an event specific processor (only transfers)
            // it is using the an event specific filter to get all Transfers
            // the node will return transfers for other contracts but 
            // we'll do the extra address filtering in the criteria
            var processor = web3.Processing.Logs.CreateProcessor<TransferEvent>(
                blockProgressRepository: blockProgressRepository,
                action: action,
                criteria: criteria);

            // it may be worth experimenting with the alternative below
            // it does create the contract specific get logs filter
            // therefore you would not need the criteria for address checking
            // depending on the node and the number of contracts it may work better
            // on infura with a known block range and 7590 contracts - there is no real difference
            //var processor = web3.Processing.Logs.CreateProcessorForContracts<TransferEvent>(
            //    contractAddresses: contractAddresses.ToArray(),
            //    blockProgressRepository: blockProgressRepository,
            //    action: action,
            //    criteria: criteria);

            //if we need to stop the processor mid execution - call cancel on the token source
            var cancellationTokenSource = new CancellationTokenSource();

            //crawl the required block range
            await processor.ExecuteAsync(
                toBlockNumber: new BigInteger(3000000),
                cancellationToken: cancellationTokenSource.Token,
                startAtBlockNumberIfNotProcessed: new BigInteger(2999500));

            Assert.Equal(168, balances.Count);

            // ** if you require continual processing 
            // the progress repository will control which block to start from
            // it will keep processing until the cancellation token is cancelled
            // await processor.ExecuteAsync(cancellationTokenSource.Token);

            // ** if you want to run continually - BUT start at a specific block (not 0 or wherever your progress repo is currently at)
            //await processor.ExecuteAsync(cancellationToken: cancellationTokenSource.Token, startAtBlockNumberIfNotProcessed: blockToStartAt);

        }

        private HashSet<string> LoadContractAddresses()
        {
            var contractAddresses = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var resourceName = $"{GetType().Namespace}.TestData.contracts.txt";

            using (var stream = this.GetType().Assembly.GetManifestResourceStream(resourceName))
            {
                using (var reader = new StreamReader(stream))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        contractAddresses.Add(line.Trim());
                    }
                }
            }

            return contractAddresses;
        }
    }
}


