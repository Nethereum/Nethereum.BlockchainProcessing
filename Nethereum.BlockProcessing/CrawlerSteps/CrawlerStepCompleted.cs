using System.Collections.Generic;
using Nethereum.BlockchainProcessing.Common.Processing;

namespace Nethereum.BlockchainProcessing.Processors
{
    public class CrawlerStepCompleted<T>
    {
        public CrawlerStepCompleted(IEnumerable<BlockchainProcessorExecutionSteps> executedStepsCollection, T stepData)
        {
            ExecutedStepsCollection = executedStepsCollection;
            StepData = stepData;
        }

        public IEnumerable<BlockchainProcessorExecutionSteps> ExecutedStepsCollection { get; private set; }
        public T StepData { get; private set; }

    }
}