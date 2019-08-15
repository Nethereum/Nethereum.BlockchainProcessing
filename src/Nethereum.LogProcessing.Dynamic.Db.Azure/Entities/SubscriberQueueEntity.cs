using System.Threading.Tasks;
using Nethereum.LogProcessing.Dynamic.Configuration;

namespace Nethereum.LogProcessing.Dynamic.Db.Azure.Entities
{

    public class SubscriberQueueEntity : SubscriberOwnedBase, ISubscriberQueueDto
    {
        public bool Disabled {get;set; }
        
        public string Name { get; set; }

    }
}
