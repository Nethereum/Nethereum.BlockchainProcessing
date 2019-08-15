using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;

namespace Nethereum.LogProcessing.Dynamic.Db.Azure.Repositories
{
    public abstract class OneToOneRepository<Dto, Entity> :
        Repository<Dto, Entity>
        where Entity : class, ITableEntity, Dto, new() where Dto : class
    {
        public OneToOneRepository(CloudTable table) : base(table)
        {
        }

        public virtual async Task<Dto> GetAsync(long id)
        {
            return await GetAsync(id.ToString(), id.ToString()).ConfigureAwait(false);
        }
    }
}
