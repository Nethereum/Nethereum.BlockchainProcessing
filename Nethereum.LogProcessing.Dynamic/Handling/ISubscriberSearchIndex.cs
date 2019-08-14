using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public interface ISubscriberSearchIndex
    {
        string Name {get;}
        Task IndexAsync(DecodedEvent decodedEvent);
    }
}
