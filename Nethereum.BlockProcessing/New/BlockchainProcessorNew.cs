using Common.Logging;
using Nethereum.BlockchainProcessing.Processors;
using Nethereum.RPC.Eth.Blocks;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Nethereum.BlockchainProcessing.Processing
{
    public class BlockchainProcessorNew
    {
        private IBlockchainProcessingOrchestrator _blockchainProcessingOrchestrator;
        private IBlockProgressRepository _blockProgressRepository;
        private ILastConfirmedBlockNumberService _lastConfirmedBlockNumberService;
        private ILog _log;

        public BlockchainProcessorNew(IBlockchainProcessingOrchestrator blockchainProcessingOrchestrator, IBlockProgressRepository blockProgressRepository, ILastConfirmedBlockNumberService lastConfirmedBlockNumberService, ILog log)
        {
            _blockchainProcessingOrchestrator = blockchainProcessingOrchestrator;
            _blockProgressRepository = blockProgressRepository;
            _lastConfirmedBlockNumberService = lastConfirmedBlockNumberService;
            _log = log;
        }

        //Scenario I have a repository and want to restart from last one, til cancellation
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var lastProcessedNumber = await _blockProgressRepository.GetLastBlockNumberProcessedAsync();
            var fromBlockNumber = GetNextMinimumBlockNumber(lastProcessedNumber);

            await ExecuteAsync(fromBlockNumber, cancellationToken);
        }

        //Scenario I have a repository and want to start from a number, til cancellation
        public async Task ExecuteAsync(BigInteger fromBlockNumber, CancellationToken cancellationToken)
        {

            while (!cancellationToken.IsCancellationRequested)
            {
                var blockToProcess = await _lastConfirmedBlockNumberService.GetLastConfirmedBlockNumberAsync(fromBlockNumber, cancellationToken);
                var progress = await _blockchainProcessingOrchestrator.ProcessAsync(fromBlockNumber, blockToProcess);
                if (!progress.HasErrored)
                {
                    fromBlockNumber = GetNextMinimumBlockNumber(progress.BlockNumberProcessTo);
                    LastBlockProcessed(progress.BlockNumberProcessTo);
                }
                else
                {
                    throw progress.Exception;
                }
            }
        }

        public async Task ExecuteAsync(BigInteger fromBlockNumber, BigInteger toBlockNumber, CancellationToken cancellationToken)
        {

            while (!cancellationToken.IsCancellationRequested && fromBlockNumber <= toBlockNumber)
            {
                var blockToProcess = await _lastConfirmedBlockNumberService.GetLastConfirmedBlockNumberAsync(fromBlockNumber, cancellationToken);
                if (blockToProcess > toBlockNumber) blockToProcess = toBlockNumber;

                var progress = await _blockchainProcessingOrchestrator.ProcessAsync(fromBlockNumber, blockToProcess);
                if (!progress.HasErrored)
                {
                    fromBlockNumber = GetNextMinimumBlockNumber(progress.BlockNumberProcessTo);
                    LastBlockProcessed(progress.BlockNumberProcessTo);
                }
                else
                {
                    throw progress.Exception;
                }
            }
        }

        public BigInteger GetNextMinimumBlockNumber(BigInteger? lastProcessedNumber)
        {
            var mininumBlockNumber = lastProcessedNumber ?? 0;
            if (mininumBlockNumber > 0) mininumBlockNumber = mininumBlockNumber + 1; //start at next block
            return mininumBlockNumber;
        }

        private void LastBlockProcessed(BigInteger? lastBlock)
        {
            _log?.Info(lastBlock == null ? "No blocks previously processed" : $"Last Block: {lastBlock}");
        }
    }
}