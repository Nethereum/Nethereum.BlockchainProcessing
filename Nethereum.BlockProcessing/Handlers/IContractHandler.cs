using System.Threading.Tasks;
using Nethereum.BlockProcessing.ValueObjects;

namespace Nethereum.BlockchainProcessing.Handlers
{

    public interface IContractHandler
    {
        Task HandleAsync(ContractTransaction contractTransaction);
        Task<bool> ExistsAsync(string contractAddress);
    }
}
