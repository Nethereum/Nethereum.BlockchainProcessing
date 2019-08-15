
using Nethereum.LogProcessing.Dynamic.Configuration;
using System.Threading.Tasks;

namespace Nethereum.LogProcessing.Dynamic.Handling
{
    public interface IEventHandlerConfigurationFactory
    {
        Task<EventSubscriptionStateDto> GetEventSubscriptionStateAsync(long eventSubscriptionId);
        Task SaveAsync(EventSubscriptionStateDto state);
    }
}
