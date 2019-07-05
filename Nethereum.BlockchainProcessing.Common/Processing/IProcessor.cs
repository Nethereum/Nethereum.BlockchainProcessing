using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Common.Processing
{
    public interface IProcessor<T>
    {
        void SetMatchCriteria(Func<T, bool> criteria);
        void SetMatchCriteria(Func<T, Task<bool>> criteria);
        void AddProcessorHandler(Func<T, Task> action);
        void AddProcessorHandler(IProcessorHandler<T> processorHandler);

        Task<bool> IsMatchAsync(T value);
        Task ExecuteAsync(T value);
    }
}