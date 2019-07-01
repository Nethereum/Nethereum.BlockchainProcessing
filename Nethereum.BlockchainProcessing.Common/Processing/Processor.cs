using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Common.Processing
{
    public class Processor<T>: IProcessor<T>
    {
        public Func<T, Task<bool>> Criteria { get; protected set; }
        protected List<Func<Task<T>>> ProcessorHandlers { get; set; } = new List<Func<Task<T>>>();
        public virtual void SetMatchCriteria(Func<T, bool> criteria)
        {
            Func<T, Task<bool>> asyncCriteria = async (t) => criteria(t);

            SetMatchCriteria(asyncCriteria);
        }

        public virtual void SetMatchCriteria(Func<T, Task<bool>> criteria)
        {
            Criteria = criteria;
        }

        public virtual void AddProcessorHandler(Func<Task<T>> action)
        {
            ProcessorHandlers.Add(action);
        }

        public virtual Task<bool> IsMatchAsync(T value)
        {
            return Criteria(value);
        }

        public virtual Task ExecuteAsync(T value)
        {
            //TODO: JB
            throw new NotImplementedException();
        }

        public virtual Task ExecuteParallelAsync(T value)
        {
            //TODO: JB
            throw new NotImplementedException();
        }
    }
}