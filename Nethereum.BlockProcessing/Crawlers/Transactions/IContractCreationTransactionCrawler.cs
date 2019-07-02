using System.Threading.Tasks;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainProcessing.Processors.Transactions
{
    public interface IContractCreationTransactionCrawler
    {
        Task ProcessTransactionAsync(
            Transaction transaction, 
            TransactionReceipt transactionReceipt, 
            HexBigInteger blockTimestamp);
    }
}