
using System.Threading.Tasks;

namespace Nethereum.LogProcessing.Dynamic.Configuration
{
    public interface IEventHandlerRepository
    {
        Task<IEventHandlerDto[]> GetManyAsync(long eventSubscriptionId);
        Task<IEventHandlerDto> UpsertAsync(IEventHandlerDto dto);
    }

}
