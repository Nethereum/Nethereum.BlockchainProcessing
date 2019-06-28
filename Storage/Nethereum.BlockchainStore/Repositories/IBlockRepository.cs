using Nethereum.BlockchainStore.Entities;
using Nethereum.Hex.HexTypes;
using System.Numerics;
using System.Threading.Tasks;

namespace Nethereum.BlockchainStore.Repositories
{
    public interface IBlockRepository
    {
        Task UpsertBlockAsync(Nethereum.RPC.Eth.DTOs.Block source);
        Task<IBlockView> FindByBlockNumberAsync(HexBigInteger blockNumber);
    }
}