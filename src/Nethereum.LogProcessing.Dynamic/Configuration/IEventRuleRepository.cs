using System.Threading.Tasks;

namespace Nethereum.LogProcessing.Dynamic.Configuration
{
    public interface IEventRuleRepository
    {
        Task<IEventRuleDto> GetAsync(long eventHandlerId);
        Task<IEventRuleDto> UpsertAsync(IEventRuleDto dto);
    }
}
