
using System.Threading.Tasks;

namespace Nethereum.LogProcessing.Dynamic.Configuration
{
    public interface ISubscriberRepository
    {
        Task<ISubscriberDto[]> GetManyAsync(long partitionId);

        Task<ISubscriberDto> UpsertAsync(ISubscriberDto subscriber);
    }
}