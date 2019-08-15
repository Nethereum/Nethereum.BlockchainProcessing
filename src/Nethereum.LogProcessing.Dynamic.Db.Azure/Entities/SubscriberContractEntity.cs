using Nethereum.LogProcessing.Dynamic.Configuration;

namespace Nethereum.LogProcessing.Dynamic.Db.Azure.Entities
{
    public class SubscriberContractEntity : SubscriberOwnedBase, ISubscriberContractDto
    {
        public string Abi { get;set;}
        public string Name { get; set; }

    }
}
