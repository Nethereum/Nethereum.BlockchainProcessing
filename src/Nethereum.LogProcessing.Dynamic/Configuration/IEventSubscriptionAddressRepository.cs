using System.Threading.Tasks;

namespace Nethereum.LogProcessing.Dynamic.Configuration
{
    public interface IEventSubscriptionAddressRepository
    {
        Task<IEventSubscriptionAddressDto[]> GetManyAsync(long eventSubscriptionId);
        Task<IEventSubscriptionAddressDto> UpsertAsync(IEventSubscriptionAddressDto dto);
    }
}
