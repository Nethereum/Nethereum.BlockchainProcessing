using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.LogProcessing.Dynamic.Configuration;
using Nethereum.LogProcessing.Dynamic.Db.Azure.Entities;

namespace Nethereum.LogProcessing.Dynamic.Db.Azure.Repositories
{
    public class SubscriberStorageRepository :
        SubscriberOwnedRepository<ISubscriberStorageDto, SubscriberStorageEntity>, ISubscriberStorageRepository
    {
        public SubscriberStorageRepository(CloudTable table) : base(table)
        {
        }

        protected override SubscriberStorageEntity Map(ISubscriberStorageDto dto)
        {
            return new SubscriberStorageEntity
            {
                Disabled = dto.Disabled,
                Id = dto.Id,
                Name = dto.Name,
                SubscriberId = dto.SubscriberId
            };
        }
    }
}
