using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.LogProcessing.Dynamic.Configuration;
using Nethereum.LogProcessing.Dynamic.Db.Azure.Entities;

namespace Nethereum.LogProcessing.Dynamic.Db.Azure.Repositories
{
    public class ContractQueryRepository : OneToOneRepository<IContractQueryDto, ContractQueryEntity>, IContractQueryRepository
    {
        public ContractQueryRepository(CloudTable table) : base(table)
        {
        }

        protected override ContractQueryEntity Map(IContractQueryDto dto)
        {
            return new ContractQueryEntity
            {
                Id = dto.EventHandlerId,
                ContractAddress = dto.ContractAddress,
                ContractAddressParameterNumber = dto.ContractAddressParameterNumber,
                ContractAddressSource = dto.ContractAddressSource,
                ContractAddressStateVariableName = dto.ContractAddressStateVariableName,
                ContractId = dto.ContractId,
                EventHandlerId = dto.EventHandlerId,
                EventStateOutputName = dto.EventStateOutputName,
                FunctionSignature = dto.FunctionSignature,
                SubscriptionStateOutputName = dto.SubscriptionStateOutputName
            };
        }
    }
}
