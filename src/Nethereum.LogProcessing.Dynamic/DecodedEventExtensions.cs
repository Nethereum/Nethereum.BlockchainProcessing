using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.Model;
using Nethereum.Contracts;
using Nethereum.LogProcessing.Dynamic.Handling.Handlers;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nethereum.LogProcessing.Dynamic
{
    public static class DecodedEventExtensions
    {
        public static EventLogQueueMessage ToQueueMessage(this DecodedEvent decodedEvent)
        {
            var msg = new EventLogQueueMessage
            {
                Key = decodedEvent.Key,
                Event = decodedEvent.DecodedEventDto,
                State = decodedEvent.State,
                Transaction = decodedEvent.Transaction,
                Log = decodedEvent.Log,
                ParameterValues = decodedEvent.Event.Select(p => new EventParameterValue
                {
                    Order = p.Parameter.Order,
                    AbiType = p.Parameter.ABIType.Name,
                    Name = p.Parameter.Name,
                    Value = p.Result,
                    Indexed = p.Parameter.Indexed
                }).ToList()
            };

            return msg;
        }

        public static TEventDto GetDecodedEventDto<TEventDto>(this EventLogQueueMessage msg) where TEventDto : class, new()
        {
            if (msg.Event == null) return null;
            if (msg.Event is TEventDto dto) return dto;
            if (msg.Event is JObject jObject) return jObject.ToObject<TEventDto>();
            if (msg.Log != null) return msg.Log.DecodeEvent<TEventDto>()?.Event;

            return null;
        }

        public static DecodedEvent ToDecodedEvent(this FilterLog log, EventABI abi = null)
        {
            var decodedParameterOutputs = abi?.DecodeEventDefaultTopics(log) ??
                new EventLog<List<ParameterOutput>>(new List<ParameterOutput>(), log);

            var decodedEvent = new DecodedEvent(decodedParameterOutputs.Event, decodedParameterOutputs.Log);
            decodedEvent.AddStateData(abi, log);
            return decodedEvent;
        }

        public static DecodedEvent ToDecodedEvent<TEvent>(this FilterLog log, EventABI abi) where TEvent : new()
        {
            var decodedParameterOutputs = abi.DecodeEventDefaultTopics(log);

            var decodedDto = log.DecodeEvent<TEvent>();

            var decodedEvent = new DecodedEvent(decodedParameterOutputs.Event, decodedParameterOutputs.Log, decodedDto.Event);
            decodedEvent.AddStateData(abi, log);
            return decodedEvent;
        }

        private static void AddStateData(this DecodedEvent decodedEvent, EventABI abi, FilterLog log)
        {
            decodedEvent.State["EventAbiName"] = abi?.Name;
            decodedEvent.State["EventSignature"] = abi?.Sha3Signature;
            decodedEvent.State["TransactionHash"] = log.TransactionHash;
            decodedEvent.State["LogIndex"] = log.LogIndex?.Value;
            decodedEvent.State["HandlerInvocations"] = 0;
            decodedEvent.State["UtcNowMs"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
    }
}
