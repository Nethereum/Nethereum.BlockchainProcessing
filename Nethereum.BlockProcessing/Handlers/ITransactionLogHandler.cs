using System.Threading.Tasks;
using Nethereum.BlockProcessing.ValueObjects;

namespace Nethereum.BlockchainProcessing.Handlers
{
    public interface ITransactionLogHandler
    {
        Task HandleAsync(TransactionLogWrapper transactionLog);
    }

    public interface ITransactionLogHandler<TEvent> : ITransactionLogHandler where TEvent : new(){

    }
}
