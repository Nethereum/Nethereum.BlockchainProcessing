using System;
using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.BlockProcessing.ValueObjects;

namespace Nethereum.BlockProcessing.Filters.Transactions
{
    public class TransactionLogFilter : Filter<TransactionLogWrapper>, ITransactionLogFilter
    {
        public TransactionLogFilter(){}

        public TransactionLogFilter(Func<TransactionLogWrapper, Task<bool>> condition) : base(condition)
        {
        }

        public TransactionLogFilter(Func<TransactionLogWrapper, bool> condition) : base(condition)
        {
        }
    }
}
