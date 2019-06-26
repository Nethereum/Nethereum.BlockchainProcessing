using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockProcessing.Filters.Transactions
{
    public interface ITransactionAndReceiptFilter: IFilter<(Transaction, TransactionReceipt)>
    {
    }
}
