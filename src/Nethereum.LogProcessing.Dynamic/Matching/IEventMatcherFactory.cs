
using Nethereum.LogProcessing.Dynamic.Configuration;
using System.Threading.Tasks;

namespace Nethereum.LogProcessing.Dynamic.Matching
{
    public interface IEventMatcherFactory
    {
        Task<IEventMatcher> LoadAsync(IEventSubscriptionDto eventSubscription);
    }
}
