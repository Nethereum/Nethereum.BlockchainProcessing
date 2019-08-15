using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.LogProcessing.Dynamic.Configuration;
using Nethereum.LogProcessing.Dynamic.Db.Azure.Entities;

namespace Nethereum.LogProcessing.Dynamic.Db.Azure.Repositories
{
    public class SubscriberContractsRepository : 
        SubscriberOwnedRepository<ISubscriberContractDto, SubscriberContractEntity>, 
        ISubscriberContractRepository
    {
        public SubscriberContractsRepository(CloudTable table) : base(table)
        {
        }

        protected override SubscriberContractEntity Map(ISubscriberContractDto dto)
        {
            if (dto is SubscriberContractEntity entity) return entity;

            return new SubscriberContractEntity
            {
                Id = dto.Id,
                SubscriberId = dto.SubscriberId,
                Abi = dto.Abi,
                Name = dto.Name
            };
        }
    }
}
