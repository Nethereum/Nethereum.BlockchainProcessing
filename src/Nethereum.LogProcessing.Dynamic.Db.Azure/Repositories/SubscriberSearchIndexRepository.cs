using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.LogProcessing.Dynamic.Configuration;
using Nethereum.LogProcessing.Dynamic.Db.Azure.Entities;

namespace Nethereum.LogProcessing.Dynamic.Db.Azure.Repositories
{
    public class SubscriberSearchIndexRepository :
        SubscriberOwnedRepository<ISubscriberSearchIndexDto, SubscriberSearchIndexEntity>,
        ISubscriberSearchIndexRepository
    {
        public SubscriberSearchIndexRepository(CloudTable table) : base(table)
        {
        }

        protected override SubscriberSearchIndexEntity Map(ISubscriberSearchIndexDto dto)
        {
            return new SubscriberSearchIndexEntity
            {
                Disabled = dto.Disabled,
                Id = dto.Id,
                Name = dto.Name,
                SubscriberId = dto.SubscriberId
            };
        }
    }
}
