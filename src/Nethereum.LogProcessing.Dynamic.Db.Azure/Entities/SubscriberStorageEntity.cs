using Nethereum.LogProcessing.Dynamic.Configuration;

namespace Nethereum.LogProcessing.Dynamic.Db.Azure.Entities
{
    public class SubscriberStorageEntity : SubscriberOwnedBase, ISubscriberStorageDto
    {
        public bool Disabled { get; set; }

        public string Name { get; set; }
    }
}
