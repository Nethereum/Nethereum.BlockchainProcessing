using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.LogProcessing.Dynamic.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Nethereum.LogProcessing.Dynamic.Db.Azure.Entities
{
    public class EventSubscriptionStateEntity : TableEntity, IEventSubscriptionStateDto
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

        public string ValuesAsJson
        {
            get {  return JsonConvert.SerializeObject(Values);}
            set {  Values = JsonConvert.DeserializeObject<Dictionary<string, object>>(value); }
        }

        public Dictionary<string, object> Values {get;set; } = new Dictionary<string, object>();

    }
}
