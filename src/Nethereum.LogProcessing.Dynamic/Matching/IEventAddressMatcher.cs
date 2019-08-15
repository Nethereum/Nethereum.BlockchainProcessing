using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.LogProcessing.Dynamic.Matching
{
    public interface IEventAddressMatcher
    {
        bool IsMatch(FilterLog log);
    }
}
