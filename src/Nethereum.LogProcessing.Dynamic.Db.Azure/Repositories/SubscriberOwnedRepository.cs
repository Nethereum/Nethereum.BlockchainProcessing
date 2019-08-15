using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.LogProcessing.Dynamic.Configuration;

namespace Nethereum.LogProcessing.Dynamic.Db.Azure.Repositories
{
    public abstract class SubscriberOwnedRepository<Dto, Entity> :
        OwnedRepository<Dto, Entity>, ISubscriberOwnedRepository<Dto>
        where Entity : class, ITableEntity, Dto, new() where Dto : class
    {
        public SubscriberOwnedRepository(CloudTable table) : base(table)
        {
        }
    }
}
