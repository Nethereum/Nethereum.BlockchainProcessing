using System.Threading.Tasks;

namespace Nethereum.LogProcessing.Dynamic.Handling
{
    public interface ISubscriberSearchIndex
    {
        string Name {get;}
        Task IndexAsync(DecodedEvent decodedEvent);
    }
}
