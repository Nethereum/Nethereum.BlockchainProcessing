using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.LogProcessing.Dynamic.Configuration;
using Nethereum.LogProcessing.Dynamic.Db.Azure.Entities;

namespace Nethereum.LogProcessing.Dynamic.Db.Azure.Repositories
{
    public class EventRuleRepository : OneToOneRepository<IEventRuleDto, EventRuleEntity>, IEventRuleRepository
    {
        public EventRuleRepository(CloudTable table) : base(table)
        {
        }

        protected override EventRuleEntity Map(IEventRuleDto dto)
        {
            return new EventRuleEntity
            {
                Id = dto.EventHandlerId,
                EventHandlerId = dto.EventHandlerId,
                EventParameterNumber = dto.EventParameterNumber,
                Source = dto.Source,
                InputName = dto.InputName,
                Type = dto.Type,
                Value = dto.Value
            };
        }
    }
}
