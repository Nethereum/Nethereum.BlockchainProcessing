using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.LogProcessing.Dynamic.Configuration;

namespace Nethereum.LogProcessing.Dynamic.Db.Azure.Entities
{
    public class EventSubscriptionAddressEntity : TableEntity, IEventSubscriptionAddressDto
    {
        public long EventSubscriptionId
        {
            get => this.PartionKeyToLong();
            set => PartitionKey = value.ToString();
        }
        public long Id
        {
            get => this.RowKeyToLong();
            set => RowKey = value.ToString();
        }

        public string Address { get; set; }
    }
}
