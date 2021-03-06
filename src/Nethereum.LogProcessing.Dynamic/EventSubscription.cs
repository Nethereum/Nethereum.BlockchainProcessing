﻿using Nethereum.ABI.Model;
using Nethereum.BlockchainProcessing.Processor;
using Nethereum.LogProcessing.Dynamic.Configuration;
using Nethereum.LogProcessing.Dynamic.Handling;
using Nethereum.LogProcessing.Dynamic.Matching;
using Nethereum.RPC.Eth.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nethereum.LogProcessing.Dynamic
{
    public class EventSubscription<TEvent> : EventSubscription where TEvent : new()
    {
        public EventSubscription(
            long id = 0, 
            long subscriberId = 0, 
            EventSubscriptionStateDto state = null,
            string[] contractAddressesToMatch = null,
            IEventHandlerHistoryRepository eventHandlerHistoryDb = null,
            IEnumerable<IParameterCondition> parameterConditions = null)
            :base(
                 id, 
                 subscriberId, 
                 new EventMatcher<TEvent>(
                     new EventAddressMatcher(contractAddressesToMatch),
                     new EventParameterMatcher(parameterConditions)), 
                 new EventHandlerManager(eventHandlerHistoryDb), 
                 state ?? new EventSubscriptionStateDto(id))
        {
            
        }

        protected override Task ExecuteInternalAsync(FilterLog value)
        {
            State.Increment("HandlerInvocations");
            return HandlerManager.HandleAsync<TEvent>(this, Matcher.Abis.First(), value);
        }
    }

    /// <summary>
    /// a dynamically loaded subscription
    /// Designed to be instantiated from DB configuration data
    /// </summary>

    public class EventSubscription : ProcessorHandler<FilterLog>, IEventSubscription
    {
        public EventSubscription(
            EventABI[] eventAbis, 
            string[] contractAddressesToMatch = null,
            IEnumerable<IParameterCondition> parameterConditions = null,
            long id = 0, 
            long subscriberId = 0,
            IEventSubscriptionStateDto state = null,
            IEventHandlerHistoryRepository eventHandlerHistoryDb = null):
            this(
                 id,
                 subscriberId,
                 new EventMatcher(
                     eventAbis,
                     new EventAddressMatcher(contractAddressesToMatch),
                     new EventParameterMatcher(parameterConditions)),
                 new EventHandlerManager(eventHandlerHistoryDb),
                 state ?? new EventSubscriptionStateDto(id)
                )
        {
        }

        public EventSubscription(
            long id, 
            long subscriberId, 
            IEventMatcher matcher, 
            IEventHandlerManager handlerManager, 
            IEventSubscriptionStateDto state)
        {
            Id = id;
            SubscriberId = subscriberId;
            Matcher = matcher ?? throw new System.ArgumentNullException(nameof(matcher));
            HandlerManager = handlerManager ?? throw new System.ArgumentNullException(nameof(handlerManager));
            State = state;
            Handlers = new List<IEventHandler>();

            if (!State.Values.ContainsKey("HandlerInvocations"))
            {
                State.SetInt("HandlerInvocations", 0);
            }
        }

        public void AddHandler(IEventHandler handler)
        {
            Handlers.Add(handler);
        }

        public IEnumerable<IEventHandler> EventHandlers => Handlers.AsReadOnly();

        private List<IEventHandler> Handlers { get; }

        public long SubscriberId {get; }

        public long Id {get; }

        public IEventHandlerManager HandlerManager { get; }
        public IEventSubscriptionStateDto State { get; }
        public IEventMatcher Matcher { get; }

        protected override Task ExecuteInternalAsync(FilterLog value)
        {
            State.Increment("HandlerInvocations");
            return HandlerManager.HandleAsync(this, Matcher.Abis, value);
        }

        public override Task<bool> IsMatchAsync(FilterLog value)
        {
            return Task.FromResult(Matcher.IsMatch(value));
        }
    }
}
