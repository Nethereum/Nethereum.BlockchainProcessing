using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.LogProcessing.Dynamic.Configuration;
using Nethereum.LogProcessing.Dynamic.Db.Azure.Entities;

namespace Nethereum.LogProcessing.Dynamic.Db.Azure.Repositories
{
    public class EventSubscriptionAddressRepository : OwnedRepository<IEventSubscriptionAddressDto, EventSubscriptionAddressEntity>, IEventSubscriptionAddressRepository
    {
        public EventSubscriptionAddressRepository(CloudTable table) : base(table)
        {
        }

        protected override EventSubscriptionAddressEntity Map(IEventSubscriptionAddressDto dto)
        {
            return new EventSubscriptionAddressEntity
            {
                Id = dto.Id,
                EventSubscriptionId = dto.EventSubscriptionId,
                Address = dto.Address
            };
        }
    }
}
