using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.LogProcessing.Dynamic.Configuration;

namespace Nethereum.LogProcessing.Dynamic.Db.Azure.Entities
{
    public class SubscriberSearchIndexEntity : SubscriberOwnedBase, ISubscriberSearchIndexDto
    {
        public bool Disabled { get; set; }
        public string Name { get; set; }
    }
}
