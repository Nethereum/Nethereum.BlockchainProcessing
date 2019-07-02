using System;
using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.BlockProcessing.ValueObjects;

namespace Nethereum.BlockchainProcessing.InMemory.Console
{
    public class InMemoryTransactionLogHandler : InMemoryHandlerBase, ITransactionLogHandler
    {
        public InMemoryTransactionLogHandler(Action<string> logAction) : base(logAction)
        {
        }

        public Task HandleAsync(LogWithReceiptAndTransaction filterLogWithReceiptAndTransactionLog)
        {
            Log($"[TransactionLog] Hash:{filterLogWithReceiptAndTransactionLog.Transaction.TransactionHash}, Index:{filterLogWithReceiptAndTransactionLog.LogIndex}, Address:{filterLogWithReceiptAndTransactionLog.Log.Address}");
            return Task.CompletedTask;
        }
    }
}
