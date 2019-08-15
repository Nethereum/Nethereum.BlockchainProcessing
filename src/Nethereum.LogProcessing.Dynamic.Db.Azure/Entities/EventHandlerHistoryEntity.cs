using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.LogProcessing.Dynamic.Configuration;

namespace Nethereum.LogProcessing.Dynamic.Db.Azure.Entities
{
    public class EventHandlerHistoryEntity : TableEntity, IEventHandlerHistoryDto
    {
        public long EventHandlerId
        {
            get => this.PartionKeyToLong();
            set => PartitionKey = value.ToString();
        }

        public string EventKey
        {
            get => this.RowKey;
            set => RowKey = value;
        }

        public long SubscriberId { get;set; }
        public long EventSubscriptionId { get; set; }
        public long Id { get; set; }
    }
}
