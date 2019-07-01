using System.Threading.Tasks;
using Nethereum.BlockProcessing.ValueObjects;

namespace Nethereum.BlockchainProcessing.Handlers
{
    public class NullTransactionLogHandler : ITransactionLogHandler
    {
        public Task HandleAsync(FilterLogWithReceiptAndTransaction filterLogWithReceiptAndTransactionLog)
        {
            return Task.CompletedTask;
        }
    }
}