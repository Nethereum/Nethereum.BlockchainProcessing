using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.LogProcessing.Dynamic.Configuration;
using Nethereum.LogProcessing.Dynamic.Db.Azure.Entities;


namespace Nethereum.LogProcessing.Dynamic.Db.Azure.Repositories
{
    public class SubscriberQueueRepository :
        SubscriberOwnedRepository<ISubscriberQueueDto, SubscriberQueueEntity>,
        ISubscriberQueueRepository
    {
        public SubscriberQueueRepository(CloudTable table) : base(table)
        {
        }

        protected override SubscriberQueueEntity Map(ISubscriberQueueDto dto)
        {
            return new SubscriberQueueEntity
            {
                Disabled = dto.Disabled,
                Id = dto.Id,
                Name = dto.Name,
                SubscriberId = dto.SubscriberId
            };
        }
    }
}
