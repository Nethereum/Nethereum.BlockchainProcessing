using Nethereum.ABI.Model;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.LogProcessing.Dynamic.Matching
{
    public interface IEventParameterMatcher 
    {
        bool IsMatch(EventABI[] abis, FilterLog log);
    }
}
