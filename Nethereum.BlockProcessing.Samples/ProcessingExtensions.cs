//using Common.Logging;
//using Nethereum.ABI.FunctionEncoding.Attributes;
//using Nethereum.BlockchainProcessing;
//using Nethereum.BlockchainProcessing.BlockProcessing;
//using Nethereum.BlockchainProcessing.LogProcessing;
//using Nethereum.BlockchainProcessing.Processor;
//using Nethereum.BlockchainProcessing.ProgressRepositories;
//using Nethereum.BlockchainProcessing.Storage;
//using Nethereum.BlockchainProcessing.Storage.Repositories;
//using Nethereum.Contracts;
//using Nethereum.Contracts.Services;
//using Nethereum.RPC.Eth.Blocks;
//using Nethereum.RPC.Eth.DTOs;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace Nethereum.BlockProcessing.Samples
//{
//    public class EventLogProcessorHandler<TEvent> : ProcessorHandler<FilterLog> where TEvent : new()
//    {
//        Func<EventLog<TEvent>, Task> _eventAction;
//        Func<EventLog<TEvent>, Task<bool>> _eventCriteria;

//        public EventLogProcessorHandler(Func<EventLog<TEvent>, Task> action):this(action, null)
//        {
//        }

//        public EventLogProcessorHandler(
//            Func<EventLog<TEvent>, Task> action,
//            Func<EventLog<TEvent>, Task<bool>> eventCriteria)
//        {
//            _eventAction = action;
//            _eventCriteria = eventCriteria;
//            SetMatchCriteriaForEvent();
//        }

//        public EventLogProcessorHandler(
//            Action<EventLog<TEvent>> action) : this(action, null)
//        {
//        }

//        public EventLogProcessorHandler(
//            Action<EventLog<TEvent>> action,
//            Func<EventLog<TEvent>, bool> eventCriteria)
//        {
//            _eventAction = (l) => { action(l); return Task.FromResult(0); };
//            if(eventCriteria != null)
//            { 
//                _eventCriteria = async (l) => { return await Task.FromResult(eventCriteria(l)); };
//            }
//            SetMatchCriteriaForEvent();
//        }

//        private void SetMatchCriteriaForEvent()
//        {
//            base.SetMatchCriteria(async log =>
//            {
//                if (log.IsLogForEvent<TEvent>() == false) return false;

//                if(_eventCriteria == null) return true;

//                var eventLog = log.DecodeEvent<TEvent>();
//                return await _eventCriteria(eventLog).ConfigureAwait(false);
//            });
//        }

//        protected override Task ExecuteInternalAsync(FilterLog value)
//        {
//            var eventLog = value.DecodeEvent<TEvent>();
//            return _eventAction(eventLog);
//        }
//    }

//    public static class ProcessingExtensions
//    {
//        /*
//         * Logs Processor
//         *  for anything  CreateLogProcessor
//         *  for contract CreateLogProcessorForContract
//         *  for event and contract CreateLogProcessorForEventAndContract<T>
//         *  for contracts CreateLogProcessorForContracts
//         *  for one event CreateLogProcessorForEventAndContract<T>
//         *  for many events CreateLogProcessorForContract
//         *  
//         */

//        public static BlockchainProcessor CreateLogProcessorForEvent<TEventDTO>(this IEthApiContractService eth,
//            Action<EventLog<TEventDTO>> action,
//            IBlockProgressRepository blockProgressRepository = null,
//            ILog log = null) where TEventDTO : class, new() =>
//                CreateLogProcessor(eth, new[] { new EventLogProcessorHandler<TEventDTO>(action) }, new FilterInputBuilder<TEventDTO>().Build(), blockProgressRepository, log);


//        public static BlockchainProcessor CreateLogProcessorForEventAndContract<TEventDTO>(this IEthApiContractService eth,
//            string contractAddress,
//            Action<EventLog<TEventDTO>> action,
//            Func<EventLog<TEventDTO>, bool> criteria = null,
//            IBlockProgressRepository blockProgressRepository = null,
//            ILog log = null) where TEventDTO : class, new() =>
//                CreateLogProcessor(eth, new[] { new EventLogProcessorHandler<TEventDTO>(action, criteria) }, new FilterInputBuilder<TEventDTO>().Build(new[] { contractAddress }), blockProgressRepository, log);

//        public static BlockchainProcessor CreateLogProcessorForEventAndContracts<TEventDTO>(this IEthApiContractService eth,
//            string[] contractAddresses,
//            Action<EventLog<TEventDTO>> action,
//            Func<EventLog<TEventDTO>, bool> criteria = null,
//            IBlockProgressRepository blockProgressRepository = null,
//            ILog log = null) where TEventDTO : class, new() =>
//                CreateLogProcessor(eth, new[] { new EventLogProcessorHandler<TEventDTO>(action, criteria) }, new FilterInputBuilder<TEventDTO>().Build(contractAddresses), blockProgressRepository, log);

//        public static BlockchainProcessor CreateLogProcessorForEvent<TEventDTO>(this IEthApiContractService eth,
//            Func<EventLog<TEventDTO>, Task> action,
//            Func<EventLog<TEventDTO>, Task<bool>> criteria = null,
//            IBlockProgressRepository blockProgressRepository = null,
//            ILog log = null) where TEventDTO : class, new() =>
//                CreateLogProcessor(eth, new[] { new EventLogProcessorHandler<TEventDTO>(action, criteria) }, new FilterInputBuilder<TEventDTO>().Build(), blockProgressRepository, log);


//        public static BlockchainProcessor CreateLogProcessorForEventAndContract<TEventDTO>(this IEthApiContractService eth,
//            string contractAddress,
//            Func<EventLog<TEventDTO>, Task> action,
//            Func<EventLog<TEventDTO>, Task<bool>> criteria = null,
//            IBlockProgressRepository blockProgressRepository = null,
//            ILog log = null) where TEventDTO : class, new() =>
//                CreateLogProcessor(eth, new[] { new EventLogProcessorHandler<TEventDTO>(action, criteria) }, new FilterInputBuilder<TEventDTO>().Build(new[] { contractAddress }), blockProgressRepository, log);

//        public static BlockchainProcessor CreateLogProcessorForEventAndContracts<TEventDTO>(this IEthApiContractService eth,
//            string[] contractAddresses,
//            Func<EventLog<TEventDTO>, Task> action,
//            Func<EventLog<TEventDTO>, Task<bool>> criteria = null,
//            IBlockProgressRepository blockProgressRepository = null,
//            ILog log = null) where TEventDTO : class, new() =>
//                CreateLogProcessor(eth, new[] { new EventLogProcessorHandler<TEventDTO>(action, criteria) }, new FilterInputBuilder<TEventDTO>().Build(contractAddresses), blockProgressRepository, log);

//        public static BlockchainProcessor CreateLogProcessorForContracts<TEventDTO>(this IEthApiContractService eth,
//            ProcessorHandler<FilterLog> logProcessor,
//            string[] contractAddresses,
//            IBlockProgressRepository blockProgressRepository = null,
//            ILog log = null) where TEventDTO: class =>
//                CreateLogProcessor(eth, new[] { logProcessor }, new FilterInputBuilder<TEventDTO>().Build(contractAddresses), blockProgressRepository, log);

//        public static BlockchainProcessor CreateLogProcessorForContract(
//            this IEthApiContractService eth,
//            string contractAddress,
//            Action<FilterLog> action,
//            Func<FilterLog, bool> criteria = null,
//            IBlockProgressRepository blockProgressRepository = null,
//            ILog log = null) => CreateLogProcessor(eth, new[] { new ProcessorHandler<FilterLog>(action, criteria) }, new NewFilterInput { Address = new[]{ contractAddress } }, blockProgressRepository, log);

//        public static BlockchainProcessor CreateLogProcessorForContracts(
//            this IEthApiContractService eth,
//            string[] contractAddresses,
//            Action<FilterLog> action,
//            Func<FilterLog, bool> criteria = null,
//            IBlockProgressRepository blockProgressRepository = null,
//            ILog log = null) => CreateLogProcessor(eth, new[] { new ProcessorHandler<FilterLog>(action, criteria) }, new NewFilterInput { Address = contractAddresses }, blockProgressRepository, log);


//        //sync action and criter
//        public static BlockchainProcessor CreateLogProcessor(
//            this IEthApiContractService eth,
//            Action<FilterLog> action,
//            Func<FilterLog, bool> criteria = null,
//            NewFilterInput filter = null,
//            IBlockProgressRepository blockProgressRepository = null,
//            ILog log = null) => CreateLogProcessor(eth, new[] { new ProcessorHandler<FilterLog>(action, criteria) }, filter, blockProgressRepository, log);

//        //async action and criteria
//        public static BlockchainProcessor CreateLogProcessor(
//            this IEthApiContractService eth,
//            Func<FilterLog, Task> action,
//            Func<FilterLog, Task<bool>> criteria = null,
//            NewFilterInput filter = null,
//            IBlockProgressRepository blockProgressRepository = null,
//            ILog log = null) => CreateLogProcessor(eth, new[] { new ProcessorHandler<FilterLog>(action, criteria) }, filter, blockProgressRepository, log);

//        //single processor
//        public static BlockchainProcessor CreateLogProcessor(
//            this IEthApiContractService eth,
//            ProcessorHandler<FilterLog> logProcessor,
//            NewFilterInput filter = null,
//            IBlockProgressRepository blockProgressRepository = null,
//            ILog log = null) => CreateLogProcessor(eth, new[] { logProcessor }, filter, blockProgressRepository, log);
        
//        //multi processor
//        public static BlockchainProcessor CreateLogProcessor(
//            this IEthApiContractService eth,
//            IEnumerable<ProcessorHandler<FilterLog>> logProcessors,
//            NewFilterInput filter = null,
//            IBlockProgressRepository blockProgressRepository = null,
//            ILog log = null)
//        {
//            var orchestrator = new LogOrchestrator(eth, logProcessors, filter);

//            var progressRepository = blockProgressRepository ?? new InMemoryBlockchainProgressRepository(lastBlockProcessed: null);
//            var lastConfirmedBlockNumberService = new LastConfirmedBlockNumberService(eth.Blocks.GetBlockNumber);

//            return new BlockchainProcessor(orchestrator, progressRepository, lastConfirmedBlockNumberService, log);
//        }

//        public static BlockchainProcessor CreateBlockProcessor(
//            this IEthApiContractService eth,
//            Action<BlockProcessingSteps> configureSteps = null,
//            IBlockProgressRepository blockProgressRepository = null)
//        {
//            var processingSteps = new BlockProcessingSteps();
//            var orchestrator = new BlockCrawlOrchestrator(eth, new[] { processingSteps });

//            return CreateBlockProcessor(eth, configureSteps, blockProgressRepository, processingSteps);
//        }

//        public static BlockchainProcessor CreateBlockStorageProcessor(
//            this IEthApiContractService eth,
//            IBlockchainStoreRepositoryFactory blockchainStorageFactory,
//            Action<BlockProcessingSteps> configureSteps = null,
//            IBlockProgressRepository blockProgressRepository = null)
//        {
//            var processingSteps = new StorageBlockProcessingSteps(blockchainStorageFactory);
//            var orchestrator = new BlockCrawlOrchestrator(eth, new[] { processingSteps });

//            if(blockProgressRepository == null && blockchainStorageFactory is IBlockProgressRepositoryFactory progressRepoFactory)
//            {
//                blockProgressRepository = progressRepoFactory.CreateBlockProgressRepository();
//            }

//            return CreateBlockProcessor(eth, configureSteps, blockProgressRepository, processingSteps);
//        }

//        private static BlockchainProcessor CreateBlockProcessor(IEthApiContractService eth, Action<BlockProcessingSteps> configureSteps, IBlockProgressRepository blockProgressRepository, BlockProcessingSteps processingSteps)
//        {
//            var orchestrator = new BlockCrawlOrchestrator(eth, new[] { processingSteps });
//            var progressRepository = blockProgressRepository ?? new InMemoryBlockchainProgressRepository(lastBlockProcessed: null);
//            var lastConfirmedBlockNumberService = new LastConfirmedBlockNumberService(eth.Blocks.GetBlockNumber);

//            configureSteps?.Invoke(processingSteps);

//            return new BlockchainProcessor(orchestrator, progressRepository, lastConfirmedBlockNumberService);
//        }

//        public static void AddSynchronousProcessorHandler<T>(this IProcessor<T> processor, Action<T> action)
//        {
//            processor.AddProcessorHandler(t => { action(t); return Task.CompletedTask; });
//        }

//    }
//}


