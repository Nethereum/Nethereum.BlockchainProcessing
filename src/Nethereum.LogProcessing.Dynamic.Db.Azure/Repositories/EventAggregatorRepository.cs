using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.LogProcessing.Dynamic.Configuration;
using Nethereum.LogProcessing.Dynamic.Db.Azure.Entities;

namespace Nethereum.LogProcessing.Dynamic.Db.Azure.Repositories
{

    public class EventAggregatorRepository : 
        OneToOneRepository<IEventAggregatorDto, EventAggregatorEntity>, IEventAggregatorRepository
    {
        public EventAggregatorRepository(CloudTable table) : base(table)
        {
        }

        protected override EventAggregatorEntity Map(IEventAggregatorDto dto)
        {
            return new EventAggregatorEntity
            {
                Id = dto.EventHandlerId,
                EventHandlerId = dto.EventHandlerId,
                Destination = dto.Destination,
                EventParameterNumber = dto.EventParameterNumber,
                Operation = dto.Operation,
                OutputKey = dto.OutputKey,
                Source = dto.Source,
                SourceKey = dto.SourceKey
            };
        }
    }
}
