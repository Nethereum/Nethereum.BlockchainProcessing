
using Nethereum.LogProcessing.Dynamic.Configuration;
using System.Threading.Tasks;

namespace Nethereum.LogProcessing.Dynamic.Handling
{
    public interface ISubscriberSearchIndexFactory
    {
        Task<ISubscriberSearchIndex> GetSubscriberSearchIndexAsync(ISubscriberSearchIndexDto dto);
    }
}
