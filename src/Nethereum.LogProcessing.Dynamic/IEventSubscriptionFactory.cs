using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.LogProcessing.Dynamic
{
    public interface IEventSubscriptionFactory
    {
        Task<List<EventSubscription>> LoadAsync(long partitionId);
    }
}
