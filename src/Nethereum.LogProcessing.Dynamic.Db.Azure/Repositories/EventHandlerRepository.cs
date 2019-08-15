using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.LogProcessing.Dynamic.Configuration;
using Nethereum.LogProcessing.Dynamic.Db.Azure.Entities;

namespace Nethereum.LogProcessing.Dynamic.Db.Azure.Repositories
{
    public class EventHandlerRepository : OwnedRepository<IEventHandlerDto, EventHandlerEntity>, IEventHandlerRepository
    {
        public EventHandlerRepository(CloudTable table) : base(table)
        {
        }

        protected override EventHandlerEntity Map(IEventHandlerDto dto)
        {
            return new EventHandlerEntity
            {
                Id = dto.Id,
                EventSubscriptionId = dto.EventSubscriptionId,
                Disabled = dto.Disabled,
                HandlerType = dto.HandlerType,
                Order = dto.Order,
                SubscriberQueueId = dto.SubscriberQueueId,
                SubscriberRepositoryId = dto.SubscriberRepositoryId,
                SubscriberSearchIndexId = dto.SubscriberSearchIndexId
            };
        }
    }
}
