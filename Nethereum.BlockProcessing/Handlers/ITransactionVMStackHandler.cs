using System.Threading.Tasks;
using Nethereum.BlockProcessing.ValueObjects;

namespace Nethereum.BlockchainProcessing.Handlers
{
    public interface ITransactionVMStackHandler
    {
        Task HandleAsync(TransactionVmStack transactionVmStack);
    }
}
