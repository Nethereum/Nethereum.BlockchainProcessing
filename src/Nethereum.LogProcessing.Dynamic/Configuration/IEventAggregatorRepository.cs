using System.Threading.Tasks;

namespace Nethereum.LogProcessing.Dynamic.Configuration
{
    public interface IEventAggregatorRepository
    {
        Task<IEventAggregatorDto> GetAsync(long eventHandlerId);
        Task<IEventAggregatorDto> UpsertAsync(IEventAggregatorDto dto);
    }
}
