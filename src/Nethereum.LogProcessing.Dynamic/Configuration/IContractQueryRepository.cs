using System.Threading.Tasks;

namespace Nethereum.LogProcessing.Dynamic.Configuration
{
    public interface IContractQueryRepository
    {
        Task<IContractQueryDto> GetAsync(long eventHandlerId);
        Task<IContractQueryDto> UpsertAsync(IContractQueryDto dto);
    }


}
