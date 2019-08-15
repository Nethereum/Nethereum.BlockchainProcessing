using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.LogProcessing.Dynamic.Configuration
{
    public interface IEventSubscriptionStateRepository
    {
        Task<IEventSubscriptionStateDto> GetAsync(long eventSubscriptionId);
        Task UpsertAsync(IEnumerable<IEventSubscriptionStateDto> state);
    }
}
