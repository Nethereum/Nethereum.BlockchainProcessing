using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing;
using Nethereum.BlockchainProcessing.ProgressRepositories;
using Nethereum.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockProcessing.Samples
{
    public class LogProcessingForThousandsOfContracts
    {

        [Event("Transfer")]
        public class TransferEventDTO: IEventDTO
        {
            [Parameter("address", "_from", 1, true)]
            public string From { get; set; }

            [Parameter("address", "_to", 2, true)]
            public string To { get; set; }

            [Parameter("uint256", "_value", 3, false)]
            public BigInteger Value { get; set; }
        }

        [InlineData(false)]
        [InlineData(true)]
        [Theory]
        public async Task ProcessingTransfers(bool useContractAddressFilter)
        {
            // Scenario:
            // We want to track transfers for thousands of contract addresses

            // for the test - these addresses are coming from a file
            // obviously you can replace this with your own implementation
            // 7590 contract addresses expected
            HashSet<string> contractAddresses = LoadContractAddresses();

            // instantiate our web3 object
            var web3 = new Web3.Web3(TestConfiguration.BlockchainUrls.Infura.Rinkeby);

            // somewhere to store the balance of each account involved in a transfer
            // there's no opening balance implementation here - so some balances may become negative
            var balances = new Dictionary<string, BigInteger>();

            // the action to handle each relevant transfer event
            // this is a trivial synchronous implementation (in memory balance management)
            // your own can action can be whatever you need it to be
            // this may involve persistence, lookups and Async calls etc
            // you can pass an async action if required (new Func<EventLog<TransferEventDTO>, Task> ...)
            var action = new Action<EventLog<TransferEventDTO>>(transferEventLog =>
            {
                var from = transferEventLog.Event.From;
                var to = transferEventLog.Event.To;

                if (!balances.ContainsKey(from)) balances.Add(from, 0);
                if (!balances.ContainsKey(to)) balances.Add(to, 0);

                balances[from] = balances[from] - transferEventLog.Event.Value;
                balances[to] = balances[to] + transferEventLog.Event.Value;
            });

            // we're using an in memory progress repo which tracks the blocks processed
            // for real use - replace this with your own persistent implementation of IBlockProgressRepository
            // this allows you to run the processor continually (if required)
            // and pick up where it left off after a restart
            var blockProgressRepository = new InMemoryBlockchainProgressRepository();

            BlockchainProcessor processor = null;
            
            if(useContractAddressFilter == false)
            { 
                // as we're handling thousands of contracts -
                // we're not defaulting to the more obvious CreateProcessorForContracts method
                // under the hood, that would result in a filter containing all of the addresses
                // that filter would be sent to the node on a GetLogs RPC request
                // that large filter may cause issues depending on the node/client
                // instead we're using an event specific filter to get all Transfers
                // the node will return transfers for any contract and 
                // we'll do the extra address filtering in the criteria
                processor = web3.Processing.Logs.CreateProcessor<TransferEventDTO>(
                    blockProgressRepository: blockProgressRepository,
                    action: action,
                    criteria: (transferEventLog) => contractAddresses.Contains(transferEventLog.Log.Address));
            }
            else 
            {
                // it may be worth experimenting with the alternative CreateProcessorForContracts method below
                // under the hood it creates a contract specific get logs filter containing the addresses
                // therefore you don't need any extra criteria for address checking
                // depending on the node and the number of contracts it may work better
                // against infura with a known block range and 7590 contracts - there is no real difference
                processor = web3.Processing.Logs.CreateProcessorForContracts<TransferEventDTO>(
                    contractAddresses: contractAddresses.ToArray(),
                    blockProgressRepository: blockProgressRepository,
                    action: action);
            }

            //if we need to stop the processor mid execution - call cancel on the token source
            var cancellationTokenSource = new CancellationTokenSource();

            //crawl the required block range
            await processor.ExecuteAsync(
                toBlockNumber: new BigInteger(3000000),
                cancellationToken: cancellationTokenSource.Token,
                startAtBlockNumberIfNotProcessed: new BigInteger(2999500));

            Assert.Equal(168, balances.Count);

            // ** CONTINUAL PROCESSING
            // kick off the processor and leave it running until cancellation
            // the progress repository will control which block to start from
            // it will keep processing until the cancellation token is cancelled
            // await processor.ExecuteAsync(cancellationTokenSource.Token);

            // ** OPTION: Start At Block Number If Not Processed
            // normally your progress repo dictates the starting block (last block processed + 1)
            // however you might want to override this behaviour
            // why?:
            // - you have not processed anything previously and wish to start at a specific block number 
            // - the last block processed in your progress repo is too far behind and you wish to start at a more recent block
            // (the last block processed from your progress your repo will always win if it exceeds the "startAtBlockNumberIfNotProcessed" value)
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


