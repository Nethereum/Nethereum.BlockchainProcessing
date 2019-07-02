using System;
using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.BlockProcessing.ValueObjects;

namespace Nethereum.BlockProcessing.Filters.Transactions
{
    public class TransactionLogFilter : Filter<LogWithReceiptAndTransaction>, ITransactionLogFilter
    {
        public TransactionLogFilter(){}

        public TransactionLogFilter(Func<LogWithReceiptAndTransaction, Task<bool>> condition) : base(condition)
        {
        }

        public TransactionLogFilter(Func<LogWithReceiptAndTransaction, bool> condition) : base(condition)
        {
        }
    }
}
