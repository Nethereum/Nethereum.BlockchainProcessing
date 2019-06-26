using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.BlockProcessing.ValueObjects;

namespace Nethereum.BlockProcessing.Filters.Transactions
{
    public interface ITransactionLogFilter: IFilter<TransactionLogWrapper>{}
}