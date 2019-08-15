using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.LogProcessing.Dynamic.Configuration;
using Nethereum.LogProcessing.Dynamic.Db.Azure.Entities;

namespace Nethereum.LogProcessing.Dynamic.Db.Azure.Repositories
{
    public class ParameterConditionRepository : OwnedRepository<IParameterConditionDto, ParameterConditionEntity>, IParameterConditionRepository
    {
        public ParameterConditionRepository(CloudTable table) : base(table)
        {
        }

        protected override ParameterConditionEntity Map(IParameterConditionDto dto)
        {
            return new ParameterConditionEntity
            {
                Id = dto.Id,
                EventSubscriptionId = dto.EventSubscriptionId,
                Operator = dto.Operator,
                ParameterOrder = dto.ParameterOrder,
                Value = dto.Value
            };
        }
    }
}
