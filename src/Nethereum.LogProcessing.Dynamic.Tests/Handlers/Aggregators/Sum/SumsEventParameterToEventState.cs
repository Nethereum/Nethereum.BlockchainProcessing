using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.Model;
using Nethereum.LogProcessing.Dynamic.Configuration;
using System.Numerics;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.LogProcessing.Dynamic.Tests.Handlers.Aggregators.Sum
{
    public class SumsEventParameterToEventState : SumTestsBase
    {
        private const string OUTPUT_NAME = "RunningTotal";
        private const int EVENT_PARAMETER_NUMBER = 3;

        protected override IEventAggregatorDto CreateConfiguration()
        {
            return new EventAggregatorDto
            {
                Operation = AggregatorOperation.Sum,
                Source = AggregatorSource.EventParameter,
                EventParameterNumber = EVENT_PARAMETER_NUMBER,
                Destination = AggregatorDestination.EventState,
                OutputKey = OUTPUT_NAME
            };
        }

        [Fact]
        public override async Task CreatesAndIncrementsSum()
        {
            var decodedEvent = DecodedEvent.Empty();
            decodedEvent.Event.Add(
                new ParameterOutput
                {
                    Result = (BigInteger)101,
                    Parameter = new Parameter("uint256", EVENT_PARAMETER_NUMBER)
                });

            for (var i = 0; i < 3; i++)
            {
                await Aggregator.HandleAsync(decodedEvent);
            }

            Assert.Equal((BigInteger)303, decodedEvent.State[OUTPUT_NAME]);
        }

        [Fact]
        public override async Task IncrementsExistingSum()
        {
            var decodedEvent = DecodedEvent.Empty();
            decodedEvent.State[OUTPUT_NAME] = (BigInteger)202;
            decodedEvent.Event.Add(
                new ParameterOutput
                {
                    Result = (BigInteger)101,
                    Parameter = new Parameter("uint256", EVENT_PARAMETER_NUMBER)
                });

            await Aggregator.HandleAsync(decodedEvent);

            Assert.Equal((BigInteger)303, decodedEvent.State[OUTPUT_NAME]);
        }
    }
}

