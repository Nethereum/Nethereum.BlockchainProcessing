
using System.Threading.Tasks;

namespace Nethereum.LogProcessing.Dynamic.Configuration
{
    public interface IEventSubscriptionRepository
    {
        Task<IEventSubscriptionDto[]> GetManyAsync(long subscriberId);

        Task<IEventSubscriptionDto> UpsertAsync(IEventSubscriptionDto subscription);
    }
}
