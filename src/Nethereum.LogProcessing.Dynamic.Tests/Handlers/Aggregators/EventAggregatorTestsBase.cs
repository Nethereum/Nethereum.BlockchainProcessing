using Moq;
using Nethereum.LogProcessing.Dynamic.Configuration;
using Nethereum.LogProcessing.Dynamic.Handling.Handlers;

namespace Nethereum.LogProcessing.Dynamic.Tests.Handlers.Aggregators
{
    public abstract class EventAggregatorTestsBase
    {
        protected IEventAggregatorDto AggregatorConfig;
        protected EventSubscriptionStateDto EventSubscriptionState;
        protected EventAggregator Aggregator;
        protected Mock<IEventSubscription> MockEventSubscription;
        protected abstract IEventAggregatorDto CreateConfiguration();

        public EventAggregatorTestsBase()
        {
            MockEventSubscription = new Mock<IEventSubscription>();
            AggregatorConfig = CreateConfiguration();
            EventSubscriptionState = new EventSubscriptionStateDto();
            MockEventSubscription.Setup(s => s.State).Returns(EventSubscriptionState);
            Aggregator = new EventAggregator(MockEventSubscription.Object, 1, AggregatorConfig);
        }
    }
}

