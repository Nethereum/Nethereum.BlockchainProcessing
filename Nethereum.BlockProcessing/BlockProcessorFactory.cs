using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.BlockchainProcessing.Processors;
using Nethereum.BlockchainProcessing.Processors.PostProcessors;
using Nethereum.BlockchainProcessing.Processors.Transactions;
using Nethereum.BlockProcessing.Filters;
using Nethereum.Web3;

namespace Nethereum.BlockchainProcessing.Processing
{
    public static class BlockProcessorFactory
    {
        public static IBlockCrawler Create(IWeb3 web3,
            HandlerContainer handlers, FilterContainer filters = null, bool postVm = false, bool processTransactionsInParallel = true)
        {
            return Create(web3, new VmStackErrorCheckerWrapper(), handlers, filters, postVm, processTransactionsInParallel);
        }

        public static IBlockCrawler Create(
            IWeb3 web3, IVmStackErrorChecker vmStackErrorChecker, 
            HandlerContainer handlers, FilterContainer filters = null, bool postVm = false, bool processTransactionsInParallel = true)
        {

            var transactionLogProcessor = new TransactionLogProcessor(
                filters?.TransactionLogFilters,
                handlers.TransactionLogHandler);

            var contractTransactionProcessor = new ContractTransactionCrawler(
                web3, vmStackErrorChecker, handlers.ContractHandler,
                handlers.TransactionHandler, handlers.TransactionVmStackHandler);

            var contractCreationTransactionProcessor = new ContractCreationTransactionCrawler(
                web3, handlers.ContractHandler,
                handlers.TransactionHandler);

            var valueTransactionProcessor = new ValueTransactionProcessor(
                handlers.TransactionHandler);

            var transactionProcessor = new TransactionProcessor(
                web3, 
                contractTransactionProcessor,
                valueTransactionProcessor, 
                contractCreationTransactionProcessor, 
                transactionLogProcessor,
                filters?.TransactionFilters, 
                filters?.TransactionReceiptFilters,
                filters?.TransactionAndReceiptFilters);

            if (postVm)
                return new BlockVmPostCrawler(
                    web3, handlers.BlockHandler, transactionProcessor)
                {
                    ProcessTransactionsInParallel = processTransactionsInParallel
                };

            transactionProcessor.ContractTransactionCrawler.EnabledVmProcessing = false;
            return new BlockCrawler(
                web3, handlers.BlockHandler, transactionProcessor, filters?.BlockFilters)
            {
                ProcessTransactionsInParallel = processTransactionsInParallel
            };
        }
    }
}