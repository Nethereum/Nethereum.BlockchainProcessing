using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.LogProcessing.Dynamic.Configuration
{
    public interface IRepository<TDto>
    {
        Task<TDto> UpsertAsync(TDto dto);
        Task UpsertAsync(IEnumerable<TDto> dtos);
    }
}
