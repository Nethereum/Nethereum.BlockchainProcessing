using System.Threading.Tasks;

namespace Nethereum.BlockProcessing.Filters
{
    public interface IFilter<in T>
    {
        Task<bool> IsMatchAsync(T item);
    }
}
