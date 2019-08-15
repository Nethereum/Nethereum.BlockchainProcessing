using System.Threading.Tasks;

namespace Nethereum.LogProcessing.Dynamic.Tests.Handlers.Aggregators.Count
{
    public abstract class CountTestsBase: EventAggregatorTestsBase
    {
        public abstract Task CreatesAndIncrementsCounter();
        public abstract Task IncrementsExistingCounter();
    }
}

