
using System.Threading.Tasks;

namespace Nethereum.LogProcessing.Dynamic.Configuration
{
    public interface ISubscriberOwnedRepository<TDto> : IRepository<TDto>
    {
        Task<TDto> GetAsync(long subscriberId, long id);
        Task<TDto[]> GetManyAsync(long subscriberId);
    }
}
