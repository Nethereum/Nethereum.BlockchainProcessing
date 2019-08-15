using System.Threading.Tasks;

namespace Nethereum.LogProcessing.Dynamic.Tests.Handlers.Aggregators.AddToList
{
    public abstract class AddToListBase: EventAggregatorTestsBase
    {
        public abstract Task CreatesAndAddsToList();
        public abstract Task AddsToExistingList();
    }
}

