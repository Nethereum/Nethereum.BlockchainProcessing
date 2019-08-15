using Nethereum.LogProcessing.Dynamic.Handling.Handlers;
using System.Threading.Tasks;

namespace Nethereum.LogProcessing.Dynamic.Configuration
{
    public interface IEventContractQueryConfigurationRepository
    {
        Task<ContractQueryConfiguration> GetAsync(long subscriberId, long eventHandlerId);
    }
}
