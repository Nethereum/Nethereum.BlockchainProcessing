using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.LogProcessing.Dynamic.Configuration;
using Nethereum.LogProcessing.Dynamic.Db.Azure.Entities;

namespace Nethereum.LogProcessing.Dynamic.Db.Azure.Repositories
{

    public class SubscriberRepository : OwnedRepository<ISubscriberDto, SubscriberEntity>, ISubscriberRepository
    {
        public SubscriberRepository(CloudTable table) : base(table)
        {
        }

        protected override SubscriberEntity Map(ISubscriberDto dto)
        {
            return new SubscriberEntity
            {
                Id = dto.Id,
                PartitionId = dto.PartitionId,
                Disabled = dto.Disabled,
                Name = dto.Name
            };
        }
    }
}
