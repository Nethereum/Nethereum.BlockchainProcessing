

using Nethereum.LogProcessing.Dynamic.Configuration;

namespace Nethereum.LogProcessing.Dynamic.Handling.Handlers
{
    public class ContractQueryConfiguration
    {
        public IContractQueryParameterDto[] Parameters {get;set;} 

        public ISubscriberContractDto Contract { get;set;}

        public IContractQueryDto Query { get;set;}
    }
}
