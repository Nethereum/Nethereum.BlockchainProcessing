using Nethereum.LogProcessing.Dynamic.Configuration;
using Nethereum.LogProcessing.Dynamic.Db.Azure.Bootstrap;

namespace Nethereum.LogProcessing.Dynamic.Db.Azure.Repositories
{
    public class AzureEventProcessingConfigurationRepository : EventProcessingConfigurationRepository
    {
        public AzureEventProcessingConfigurationRepository(EventProcessingCloudTableSetup cloudTableSetup)
            :base(
                 cloudTableSetup.GetSubscriberRepository(),
                cloudTableSetup.GetSubscriberContractsRepository(),
                cloudTableSetup.GetEventSubscriptionsRepository(),
                cloudTableSetup.GetEventSubscriptionAddressesRepository(),
                cloudTableSetup.GetEventHandlerRepository(),
                cloudTableSetup.GetParameterConditionRepository(),
                cloudTableSetup.GetEventSubscriptionStateRepository(),
                cloudTableSetup.GetContractQueryRepository(),
                cloudTableSetup.GetContractQueryParameterRepository(),
                cloudTableSetup.GetEventAggregatorRepository(),
                cloudTableSetup.GetSubscriberQueueRepository(),
                cloudTableSetup.GetSubscriberSearchIndexRepository(),
                cloudTableSetup.GetEventHandlerHistoryRepository(),
                cloudTableSetup.GetEventRuleRepository(),
                cloudTableSetup.GetSubscriberStorageRepository())
        {

        }
    }
}
