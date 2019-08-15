using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.Model;
using Nethereum.LogProcessing.Dynamic.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.LogProcessing.Dynamic.Tests.Handlers.Aggregators.AddToList
{
    public class AddToListFromEventParameterToEventState : AddToListBase
    {
        private const string OUTPUT_NAME = "ParameterValues";
        private const int EVENT_PARAMETER_NUMBER = 3;

        protected override IEventAggregatorDto CreateConfiguration()
        {
            return new EventAggregatorDto
            {
                Operation = AggregatorOperation.AddToList,
                Source = AggregatorSource.EventParameter,
                EventParameterNumber = EVENT_PARAMETER_NUMBER,
                Destination = AggregatorDestination.EventState,
                OutputKey = OUTPUT_NAME
            };
        }

        [Fact]
        public override async Task CreatesAndAddsToList()
        {
            var decodedEvent = DecodedEvent.Empty();
            decodedEvent.Event.Add(
                new ParameterOutput
                {
                    Result = (BigInteger)101,
                    Parameter = new Parameter("uint256", EVENT_PARAMETER_NUMBER)
                });

            await Aggregator.HandleAsync(decodedEvent);

            var list = (IList)decodedEvent.State[OUTPUT_NAME];

            Assert.Single(list, (BigInteger)101);
        }

        [Fact]
        public override async Task AddsToExistingList()
        {
            var decodedEvent = DecodedEvent.Empty();
            decodedEvent.State[OUTPUT_NAME] = new List<BigInteger>(new[] { (BigInteger)202 }); ;
            decodedEvent.Event.Add(
                new ParameterOutput
                {
                    Result = (BigInteger)101,
                    Parameter = new Parameter("uint256", EVENT_PARAMETER_NUMBER)
                });

            await Aggregator.HandleAsync(decodedEvent);

            var list = (IList)decodedEvent.State[OUTPUT_NAME];

            Assert.Equal(2, list.Count);
            Assert.Equal((BigInteger)202, list[0]);
            Assert.Equal((BigInteger)101, list[1]);
        }

    }
}

