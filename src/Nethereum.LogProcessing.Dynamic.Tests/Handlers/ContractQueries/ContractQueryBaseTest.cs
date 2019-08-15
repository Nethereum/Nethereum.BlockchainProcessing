using Moq;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.Contracts;
using Nethereum.Contracts.Services;
using Nethereum.LogProcessing.Dynamic.Configuration;
using Nethereum.LogProcessing.Dynamic.Handling.Handlers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nethereum.LogProcessing.Dynamic.Tests.HandlerTests.ContractQueryTests
{
    public abstract class ContractQueryBaseTest
    {
        protected const string CONTRACT_ADDRESS = "0x78c1301520edff0bb14314c64987a71fa5efa407";
        protected const string OWNER_ADDRESS = "0x12c1301520edff0bb14314c64987a71fa5efa407";

        protected abstract object FAKE_QUERY_RESULT {get; }

        public static class SHA3_FUNCTION_SIGNATURES
        {
            public const string NAME = "06fdde03";
            public const string BALANCE_OF = "70a08231";
            public const string APPROVE = "095ea7b3";
        }
        
        public struct QueryArgs
        {
            public string ContractAddress {get;set;}
            public string Abi {get;set;}
            public string FunctionSignature {get;set;}
            public object[] FunctionInputValues {get;set;}
        }

        protected DecodedEvent decodedEvent;
        protected ContractQueryEventHandler contractQueryEventHandler;
        protected ContractQueryConfiguration queryConfig;
        protected EventSubscriptionStateDto subscriptionState;
        protected QueryArgs actualQueryArgs;
        protected Mock<IEventSubscription> MockEventSubscription;

        public ContractQueryBaseTest(ContractQueryConfiguration queryConfig)
        {
            this.queryConfig = queryConfig;

            decodedEvent = DecodedEvent.Empty();

            Web3Mock web3Mock = new Web3Mock();

            var fakeResults = new List<ParameterOutput>();
            fakeResults.Add(new ParameterOutput { Result = FAKE_QUERY_RESULT});

            subscriptionState = new EventSubscriptionStateDto();
            MockEventSubscription = new Mock<IEventSubscription>();
            MockEventSubscription.Setup(s => s.State).Returns(subscriptionState);

            contractQueryEventHandler = new ContractQueryEventHandler(MockEventSubscription.Object, 1, web3Mock.Eth, queryConfig);
            contractQueryEventHandler.QueryInterceptor = (abi, address, sig, inputs) =>
            {
                actualQueryArgs = new QueryArgs { };
                actualQueryArgs.Abi = abi;
                actualQueryArgs.ContractAddress = address;
                actualQueryArgs.FunctionSignature = sig;
                actualQueryArgs.FunctionInputValues = inputs;

                return Task.FromResult(fakeResults);
            };
        }

    }
}
