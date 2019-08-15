
using Nethereum.LogProcessing.Dynamic.Configuration;
using System.Threading.Tasks;

namespace Nethereum.LogProcessing.Dynamic.Handling
{
    public interface IEventHandlerFactory
    {
        Task<IEventHandler> LoadAsync(IEventSubscription subscription, IEventHandlerDto config);
    }
}
