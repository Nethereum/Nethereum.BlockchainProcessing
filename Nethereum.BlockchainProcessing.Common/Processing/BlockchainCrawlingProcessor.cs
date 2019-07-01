using Nethereum.BlockProcessing.ValueObjects;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainProcessing.Common.Processing
{
    public class BlockchainCrawlingProcessor
    {
        public IProcessor<Block> BlockStepProcessor = new Processor<Block>();
        public IProcessor<Transaction> TransactionStepProcessor = new Processor<Transaction>();
        public IProcessor<TransactionWithReceipt> TransactionReceiptStepProcessor = new Processor<TransactionWithReceipt>();
        public IProcessor<FilterLogWithReceiptAndTransaction> FilterLogStepProcesor = new Processor<FilterLogWithReceiptAndTransaction>();
    }
}