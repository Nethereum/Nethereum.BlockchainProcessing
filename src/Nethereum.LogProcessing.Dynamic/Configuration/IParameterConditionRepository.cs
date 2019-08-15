
using System.Threading.Tasks;

namespace Nethereum.LogProcessing.Dynamic.Configuration
{
    public interface IParameterConditionRepository
    {
        Task<IParameterConditionDto[]> GetManyAsync(long eventSubscriptionId);
        Task<IParameterConditionDto> UpsertAsync(IParameterConditionDto dto);
    }
}
