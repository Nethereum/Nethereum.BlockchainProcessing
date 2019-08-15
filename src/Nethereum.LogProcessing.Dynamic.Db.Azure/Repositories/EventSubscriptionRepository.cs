using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.LogProcessing.Dynamic.Configuration;
using Nethereum.LogProcessing.Dynamic.Db.Azure.Entities;

namespace Nethereum.LogProcessing.Dynamic.Db.Azure.Repositories
{
    public class EventSubscriptionRepository : OwnedRepository<IEventSubscriptionDto, EventSubscriptionEntity>,  IEventSubscriptionRepository
    {
        public EventSubscriptionRepository(CloudTable table) : base(table)
        {
        }

        protected override EventSubscriptionEntity Map(IEventSubscriptionDto dto)
        {
            return new EventSubscriptionEntity
            {
                Id = dto.Id,
                SubscriberId = dto.SubscriberId,
                CatchAllContractEvents = dto.CatchAllContractEvents,
                ContractId = dto.ContractId,
                Disabled = dto.Disabled,
                EventSignatures = dto.EventSignatures
            };
        }
    }
}
