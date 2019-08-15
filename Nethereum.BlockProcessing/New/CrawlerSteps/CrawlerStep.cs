using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.Common.Processing;
using Nethereum.Web3;

namespace Nethereum.BlockchainProcessing.Processors
{
    public abstract class CrawlerStep<TParentStep, TProcessStep>
    {
        //TODO: Disable step and / or handlers
        protected IWeb3 Web3 { get; }
        public CrawlerStep(
            IWeb3 web3
        )
        {
            Web3 = web3;
        }

        public abstract Task<TProcessStep> GetStepDataAsync(TParentStep parentStep);

        public virtual async Task<CrawlerStepCompleted<TProcessStep>> ExecuteStepAsync(TParentStep parentStep, IEnumerable<BlockchainProcessorExecutionSteps> executionStepsCollection)
        {
            var processStepValue = await GetStepDataAsync(parentStep);
            if (processStepValue == null) return null;
            var stepsToProcesss =
                await executionStepsCollection.FilterMatchingStepAsync(parentStep).ConfigureAwait(false);

            if (stepsToProcesss.Any())
            {
                await stepsToProcesss.ExecuteCurrentStepAsync(parentStep);
            }
            return new CrawlerStepCompleted<TProcessStep>(stepsToProcesss, processStepValue);

        }
    }
}