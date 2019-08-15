
using Nethereum.ABI.FunctionEncoding;
using Nethereum.Contracts.Services;
using Nethereum.LogProcessing.Dynamic.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nethereum.LogProcessing.Dynamic.Handling.Handlers
{

    public class ContractQueryEventHandler : EventHandlerBase, IEventHandler
    {
        public ContractQueryEventHandler(
            IEventSubscription subscription, 
            long id,
            IEthApiContractService ethApi, 
            ContractQueryConfiguration queryConfig):base(subscription, id)
        {
            EthApi = ethApi ?? throw new System.ArgumentNullException(nameof(ethApi));
            Configuration = queryConfig ?? throw new System.ArgumentNullException(nameof(queryConfig));
        }

        public IEthApiContractService EthApi { get; }
        public ContractQueryConfiguration Configuration { get; }

        public async Task<bool> HandleAsync(DecodedEvent decodedEvent)
        {
            try
            {
                var contractAddress = GetContractAddress(decodedEvent);
                if (contractAddress == null) return false;

                var functionInputs = BuildFunctionInputs(decodedEvent);

                List<ParameterOutput> result = await ExecuteQueryAsync(contractAddress, functionInputs).ConfigureAwait(false);

                var firstValue = result.FirstOrDefault()?.Result;

                if (!string.IsNullOrEmpty(Configuration.Query.EventStateOutputName))
                {
                    decodedEvent.State[Configuration.Query.EventStateOutputName] = firstValue;
                }

                if (!string.IsNullOrEmpty(Configuration.Query.SubscriptionStateOutputName))
                {
                    Subscription.State.Set(Configuration.Query.SubscriptionStateOutputName, firstValue);
                }

                return true;
            }
            catch (System.ArgumentOutOfRangeException)
            {
                return false;
            }
            catch(System.OverflowException)
            {
                return false;
            }
        }

        /// <summary>
        /// Allows a unit test to intercept ExecuteQueryAsync and return a fake result
        /// </summary>
        public Func<string, string, string, object[], Task<List<ParameterOutput>>> QueryInterceptor {get; set;}

        public virtual async Task<List<ParameterOutput>> ExecuteQueryAsync(
            string contractAddress, object[] functionInputs)
        {
            if(QueryInterceptor != null)
                return await QueryInterceptor(Configuration.Contract.Abi, contractAddress, Configuration.Query.FunctionSignature, functionInputs);

            var function = EthApi
                .GetContract(Configuration.Contract.Abi, contractAddress)
                .GetFunctionBySignature(Configuration.Query.FunctionSignature);

            var result = await function
                .CallDecodingToDefaultAsync(functionInputs)
                .ConfigureAwait(false);

            return result;
        }

        private object[] BuildFunctionInputs(DecodedEvent decodedEvent)
        {
            if(Configuration.Parameters == null || Configuration.Parameters.Length == 0)
            {
                return null;
            }
            object[] parameterValues = new object[Configuration.Parameters.Length];

            var ordered = Configuration.Parameters.OrderBy(p => p.Order).ToArray();

            for(int i = 0; i < ordered.Length; i++)
            {
                parameterValues[i] = GetFunctionInputParameterValue(ordered[i], decodedEvent);
            }

            return parameterValues;
        }

        private object GetFunctionInputParameterValue(IContractQueryParameterDto functionParameter, DecodedEvent decodedEvent)
        {
            switch (functionParameter.Source)
            {
                case EventValueSource.Static:
                    return functionParameter.Value;
                case EventValueSource.EventAddress:
                    return decodedEvent.Log.Address;
                case EventValueSource.EventParameters:
                    return GetFunctionInputFromEventParameter(functionParameter, decodedEvent);
                case EventValueSource.EventState:
                    return GetFunctionInputFromMetadata(functionParameter, decodedEvent);
                default:
                    return null;
            }
        }

        private static object GetFunctionInputFromMetadata(IContractQueryParameterDto functionParameter, DecodedEvent decodedEvent)
        {
            if(string.IsNullOrEmpty(functionParameter.EventStateName)) return null;

            return decodedEvent.State.ContainsKey(functionParameter.EventStateName) ?
                                    decodedEvent.State[functionParameter.EventStateName] : null;
        }

        private static object GetFunctionInputFromEventParameter(IContractQueryParameterDto functionParameter, DecodedEvent decodedEvent)
        {
            if(functionParameter.EventParameterNumber < 1) return null;

            return decodedEvent.Event.FirstOrDefault(p => p.Parameter.Order == functionParameter.EventParameterNumber )?.Result;
        }

        private string GetContractAddress(DecodedEvent decodedEvent)
        {
            switch (Configuration.Query.ContractAddressSource)
            {
                case ContractAddressSource.Static:
                    return Configuration.Query.ContractAddress;
                case ContractAddressSource.EventAddress:
                    return decodedEvent.Log.Address;
                case ContractAddressSource.EventParameter:
                    return GetAddressFromEventParameter(decodedEvent);
                case ContractAddressSource.EventState:
                    return GetAddressFromEventMetaData(decodedEvent);
                case ContractAddressSource.TransactionFrom:
                    return decodedEvent.Transaction?.From;
                case ContractAddressSource.TransactionTo:
                    return decodedEvent.Transaction?.To;
                default:
                    return null;

            }
        }

        private string GetAddressFromEventParameter(DecodedEvent decodedEvent)
        {
            if(Configuration.Query.ContractAddressParameterNumber > 0)
            {
                var matchingEventParameter = decodedEvent.Event.FirstOrDefault(p => p.Parameter.Order == Configuration.Query.ContractAddressParameterNumber);
                if(matchingEventParameter != null && matchingEventParameter.Result is string a)
                {
                    return a;
                }
            }
            return null;
        }

        private string GetAddressFromEventMetaData(DecodedEvent decodedEvent)
        {
            if(!string.IsNullOrEmpty(Configuration.Query.ContractAddressStateVariableName))
            {
                var addressFromMetaData = decodedEvent.State.ContainsKey(Configuration.Query.ContractAddressStateVariableName) ? 
                    decodedEvent.State[Configuration.Query.ContractAddressStateVariableName] : null;

                if(addressFromMetaData is string mS)
                {
                    return mS;
                }
            }
            return null;
        }
    }
}
