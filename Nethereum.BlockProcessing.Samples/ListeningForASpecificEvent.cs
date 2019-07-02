using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.Contracts;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.BlockProcessing.ValueObjects;
using Xunit;

namespace Nethereum.BlockProcessing.Samples
{
    public class ListeningForASpecificEvent
    {
        /*
 Solidity Contract Excerpt
    * event Transfer(address indexed _from, address indexed _to, uint256 indexed _value);
 Other contracts may have transfer events with different signatures, this won't work for those.
*/
        [Event("Transfer")]
        public class TransferEvent
        {
            [Parameter("address", "_from", 1, true)]
            public string From {get; set;}

            [Parameter("address", "_to", 2, true)]
            public string To {get; set;}

            [Parameter("uint256", "_value", 3, true)]
            public BigInteger Value {get; set;}
        }

        public class TransferEventHandler: ITransactionLogHandler<TransferEvent>
        {
            public List<(LogWithReceiptAndTransaction, EventLog<TransferEvent>)> TransferEventsHandled = 
                new List<(LogWithReceiptAndTransaction, EventLog<TransferEvent>)>();

            public List<LogWithReceiptAndTransaction> TransferEventsWithDifferentSignature = 
                new List<LogWithReceiptAndTransaction>();

            public Task HandleAsync(LogWithReceiptAndTransaction filterLogWithReceiptAndTransactionLog)
            {
                try
                {
                    if(!filterLogWithReceiptAndTransactionLog.IsForEvent<TransferEvent>()) return Task.CompletedTask;

                    var eventValues = filterLogWithReceiptAndTransactionLog.Decode<TransferEvent>();
                    TransferEventsHandled.Add((filterLogWithReceiptAndTransactionLog, eventValues));
                }
                catch (Exception)
                {
                    //Error whilst handling transaction log
                    //expected event signature may differ from the expected event.
                    TransferEventsWithDifferentSignature.Add(filterLogWithReceiptAndTransactionLog);
                }

                return Task.CompletedTask;
            }
        }

        [Fact]
        public async Task Run()
        {            
            var web3 = new Web3.Web3(TestConfiguration.BlockchainUrls.Infura.Rinkeby);
            var transferEventHandler = new TransferEventHandler();
            var handlers = new HandlerContainer{ TransactionLogHandler = transferEventHandler};

            var blockProcessor = BlockProcessorFactory.Create(
                web3, 
                handlers,
                processTransactionsInParallel: false);

            var processingStrategy = new BlockchainProcessingStrategy(blockProcessor);
            var blockchainProcessor = new BlockchainProcessor(processingStrategy);

            var result = await blockchainProcessor.ExecuteAsync(3146684, 3146684);

            Assert.True(result);

            //this is our expected event (see TransferEvent class)
            Assert.Single(transferEventHandler.TransferEventsHandled);

            //there is an event from another contract called Transfer
            //it can't be deserialized because
            //the number of indexed fields is different
            Assert.Single(transferEventHandler.TransferEventsWithDifferentSignature);
        }
    }
}
