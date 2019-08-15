using Microsoft.WindowsAzure.Storage.Table;

namespace Nethereum.LogProcessing.Dynamic.Db.Azure.Entities
{
    public abstract class SubscriberOwnedBase : TableEntity
    {
        public long SubscriberId
        {
            get => this.PartionKeyToLong();
            set => PartitionKey = value.ToString();
        }
        public long Id
        {
            get => this.RowKeyToLong();
            set => RowKey = value.ToString();
        }
    }
}
