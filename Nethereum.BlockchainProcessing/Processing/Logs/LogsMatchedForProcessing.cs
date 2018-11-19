using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public class LogsMatchedForProcessing
    {
        public LogsMatchedForProcessing(IEventLogProcessor logProcessor)
        {
            LogProcessor = logProcessor;
            MatchedLogs = new List<FilterLog>();
        }

        public IEventLogProcessor LogProcessor { get; }
        public List<FilterLog> MatchedLogs { get; }

        public void AddIfMatched(FilterLog log)
        {
            if (LogProcessor.IsLogForEvent(log))
            {
                MatchedLogs.Add(log);
            }
        }

        public async Task ProcessLogsAsync()
        {
            await LogProcessor.ProcessLogsAsync(this.MatchedLogs.ToArray());
        }
    }
}
