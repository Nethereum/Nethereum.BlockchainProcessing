using Nethereum.BlockProcessing.ValueObjects;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainProcessing.Common.Processing
{
    public class BlockchainProcessorExecutionSteps
    {
        public IProcessor<Block> BlockStepProcessor = new Processor<Block>();
        public IProcessor<TransactionWithBlock> TransactionStepProcessor = new Processor<TransactionWithBlock>();
        public IProcessor<TransactionWithReceipt> TransactionReceiptStepProcessor = new Processor<TransactionWithReceipt>();
        public IProcessor<LogWithReceiptAndTransaction> FilterLogStepProcesor = new Processor<LogWithReceiptAndTransaction>();
        public IProcessor<ContractCreationTransaction> ContractCreationStepProcessor = new Processor<ContractCreationTransaction>();
        public virtual IProcessor<T>  GetStep<T>()
        {
            var type = typeof(T);
            if (type == typeof(Block))
            {
                return (IProcessor<T>)BlockStepProcessor;
            }
            else if (type == typeof(TransactionWithBlock))
            {
                return (IProcessor<T>)TransactionStepProcessor;
            }
            else if (type == typeof(TransactionWithReceipt))
            {
                return (IProcessor<T>)TransactionReceiptStepProcessor;
            }
            else if (type == typeof(LogWithReceiptAndTransaction))
            {
                return (IProcessor<T>)FilterLogStepProcesor;
            }
            else if (type == typeof(ContractCreationTransaction))
            {
                return (IProcessor<T>)ContractCreationStepProcessor;
            }

            return null;
        }
    }
}