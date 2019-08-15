using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.LogProcessing.Dynamic.Configuration;
using Nethereum.LogProcessing.Dynamic.Db.Azure.Entities;

namespace Nethereum.LogProcessing.Dynamic.Db.Azure.Repositories
{
    public class ContractQueryParameterRepository : 
        OwnedRepository<IContractQueryParameterDto, ContractQueryParameterEntity>, 
        IContractQueryParameterRepository
    {
        public ContractQueryParameterRepository(CloudTable table) : base(table)
        {
        }

        protected override ContractQueryParameterEntity Map(IContractQueryParameterDto dto)
        {
            return new ContractQueryParameterEntity
            {
                ContractQueryId = dto.ContractQueryId,
                EventParameterNumber = dto.EventParameterNumber,
                EventStateName = dto.EventStateName,
                Id = dto.Id,
                Order = dto.Order,
                Source = dto.Source,
                Value = dto.Value
            };
        }
    }
}
