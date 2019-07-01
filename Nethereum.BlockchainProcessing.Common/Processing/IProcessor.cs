using System;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Common.Processing
{
    public interface IProcessor<T>
    {
        void SetMatchCriteria(Func<T, bool> criteria);
        void SetMatchCriteria(Func<T, Task<bool>> criteria);
        void AddProcessorHandler(Func<Task<T>> action);
        Task<bool> IsMatchAsync(T value);
        Task ExecuteAsync(T value);
    }
}