using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.Web3Abstractions;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public class BlockchainEventLogProcessor : IBlockchainProcessor
    {
        private readonly IEventLogProxy _eventLogProxy;
        private readonly IEnumerable<IEventLogProcessor> _logProcessors;

        public BlockchainEventLogProcessor(IEventLogProxy eventLogProxy, IEnumerable<IEventLogProcessor> logProcessors)
        {
            _eventLogProxy = eventLogProxy;
            _logProcessors = logProcessors;
        }

        public async Task ProcessAsync(ulong fromBlockNumber, ulong toBlockNumber)
        {
            await ProcessAsync(fromBlockNumber, toBlockNumber, new CancellationToken());
        }

        public async Task ProcessAsync(ulong fromBlockNumber, ulong toBlockNumber, CancellationToken cancellationToken)
        {
            var logs = await _eventLogProxy.GetLogs(new NewFilterInput
            {
                FromBlock = new BlockParameter(fromBlockNumber),
                ToBlock = new BlockParameter(toBlockNumber)
            });

            if (logs == null) return;

            var processingCollection = new List<LogsMatchedForProcessing>();

            foreach (var logProcessor in _logProcessors)
            {
                processingCollection.Add(new LogsMatchedForProcessing(logProcessor));
            }

            foreach (var log in logs)
            {
                foreach (var matchedForProcessing in processingCollection)
                {
                    matchedForProcessing.AddIfMatched(log);
                }
            }

            foreach (var matchedForProcessing in processingCollection)
            {
                await matchedForProcessing.ProcessLogsAsync();
            }
        }
    }
}
