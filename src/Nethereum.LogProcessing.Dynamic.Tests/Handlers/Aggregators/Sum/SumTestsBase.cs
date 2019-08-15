using System.Threading.Tasks;

namespace Nethereum.LogProcessing.Dynamic.Tests.Handlers.Aggregators.Sum
{
    public abstract class SumTestsBase: EventAggregatorTestsBase
    {
        public abstract Task CreatesAndIncrementsSum();
        public abstract Task IncrementsExistingSum();
    }
}

