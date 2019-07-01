using Nethereum.RPC.Eth.DTOs;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing.Logs
{
    public interface ILogProcessor
    {
        Task<bool> IsLogForMeAsync(FilterLog log);
        Task ProcessLogsAsync(params FilterLog[] eventLogs);
    }
}
