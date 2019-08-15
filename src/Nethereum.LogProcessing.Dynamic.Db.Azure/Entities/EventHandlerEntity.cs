using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.LogProcessing.Dynamic.Configuration;
using Nethereum.LogProcessing.Dynamic.Handling;
using System;

namespace Nethereum.LogProcessing.Dynamic.Db.Azure.Entities
{
    public class EventHandlerEntity : TableEntity, IEventHandlerDto
    {
        public long Id
        {
            get => this.RowKeyToLong();
            set => RowKey = value.ToString();
        }

        public long EventSubscriptionId
        {
            get => this.PartionKeyToLong();
            set => PartitionKey = value.ToString();
        }

        public int Order { get; set; }
        public bool Disabled { get; set; }

        public string HandlerTypeAsString
        {
            get { return HandlerType.ToString(); }
            set { HandlerType = (EventHandlerType)Enum.Parse(typeof(EventHandlerType), value); }
        }

        public EventHandlerType HandlerType { get; set; }
        public long SubscriberQueueId { get; set; }
        public long SubscriberSearchIndexId { get; set; }
        public long SubscriberRepositoryId { get; set; }

    }
}
