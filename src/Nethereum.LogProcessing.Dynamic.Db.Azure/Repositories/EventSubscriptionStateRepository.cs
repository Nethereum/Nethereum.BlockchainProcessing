using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.LogProcessing.Dynamic.Configuration;
using Nethereum.LogProcessing.Dynamic.Db.Azure.Entities;
using System.Threading.Tasks;

namespace Nethereum.LogProcessing.Dynamic.Db.Azure.Repositories
{
    public class EventSubscriptionStateRepository : OneToOneRepository<IEventSubscriptionStateDto, EventSubscriptionStateEntity>, IEventSubscriptionStateRepository
    {
        public EventSubscriptionStateRepository(CloudTable table) : base(table)
        {
        }

        public override async Task<IEventSubscriptionStateDto> GetAsync(long eventSubscriptionId)
        {
            var existingState = await GetAsync(eventSubscriptionId.ToString(), eventSubscriptionId.ToString()).ConfigureAwait(false);
            if(existingState == null)
            {
                existingState = new EventSubscriptionStateEntity
                {
                    EventSubscriptionId = eventSubscriptionId,
                    Id = eventSubscriptionId
                };
            }
            return existingState;
        }

        protected override  EventSubscriptionStateEntity Map(IEventSubscriptionStateDto dto)
        {
            return new EventSubscriptionStateEntity
            {
                EventSubscriptionId = dto.EventSubscriptionId,
                Id = dto.Id == 0 ? dto.EventSubscriptionId : dto.Id,
                Values = dto.Values
            };
        }
    }
}
