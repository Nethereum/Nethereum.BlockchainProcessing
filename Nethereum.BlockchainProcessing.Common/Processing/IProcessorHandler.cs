using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Common.Processing
{
    public interface IProcessorHandler<T>
    {
        Task ExecuteAsync(T value);
    }
}