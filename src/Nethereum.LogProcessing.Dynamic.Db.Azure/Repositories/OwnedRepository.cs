using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;

namespace Nethereum.LogProcessing.Dynamic.Db.Azure.Repositories
{

    public abstract class OwnedRepository<Dto, Entity> :
        Repository<Dto, Entity>
        where Entity : class, ITableEntity, Dto, new() where Dto : class
    {
        public OwnedRepository(CloudTable table) : base(table)
        {
        }

        public virtual async Task<Dto> GetAsync(long parentId, long id)
        {
            return await GetAsync(parentId.ToString(), id.ToString()).ConfigureAwait(false);
        }

        public virtual async Task<Dto[]> GetManyAsync(long parentId)
        {
            return await GetManyAsync(parentId.ToString()).ConfigureAwait(false);
        }
    }
}
