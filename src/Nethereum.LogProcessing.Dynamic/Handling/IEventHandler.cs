using System.Threading.Tasks;

namespace Nethereum.LogProcessing.Dynamic.Handling
{
    public interface IEventHandler
    {
        IEventSubscription Subscription { get;}
        long Id { get;}
        Task<bool> HandleAsync(DecodedEvent decodedEvent);
    }
}
