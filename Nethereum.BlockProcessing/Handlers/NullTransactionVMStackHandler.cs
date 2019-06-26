using System.Threading.Tasks;
using Nethereum.BlockProcessing.ValueObjects;

namespace Nethereum.BlockchainProcessing.Handlers
{
    public class NullTransactionVMStackHandler : ITransactionVMStackHandler
    {
        public Task HandleAsync(TransactionVmStack transactionVmStack)
        {
            return Task.CompletedTask;
        }
    }
}