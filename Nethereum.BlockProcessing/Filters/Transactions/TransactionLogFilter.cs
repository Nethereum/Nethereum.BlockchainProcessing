using System;
using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.BlockProcessing.ValueObjects;

namespace Nethereum.BlockProcessing.Filters.Transactions
{
    public class TransactionLogFilter : Filter<FilterLogWithReceiptAndTransaction>, ITransactionLogFilter
    {
        public TransactionLogFilter(){}

        public TransactionLogFilter(Func<FilterLogWithReceiptAndTransaction, Task<bool>> condition) : base(condition)
        {
        }

        public TransactionLogFilter(Func<FilterLogWithReceiptAndTransaction, bool> condition) : base(condition)
        {
        }
    }
}
