using Nethereum.BlockchainProcessing.Handlers;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainProcessing.Processors.Transactions;
using Nethereum.RPC.Eth.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.Eth.Services;
using Nethereum.Hex.HexTypes;
using Nethereum.Contracts.Services;
using Nethereum.Web3;
using System.Numerics;
using Nethereum.BlockchainProcessing.Common.Processing;
using Nethereum.BlockProcessing.Filters;
using Nethereum.BlockProcessing.ValueObjects;

namespace Nethereum.BlockchainProcessing.Processors
{

    //Logic
    //StorageProcessingHandling

    //QueueHandling but only for Address in X to X y Z
    //QueueHandling but only for Address in Y to M y Z

    //BlockchainProcessinHandling
    //BlockStepHandler(IBlockHandler, BlockStepMatchCriteria)
    //TransactionStepHandler(IContractCreationHandler, 

    /*
      public class BlockchainNavigatorProcessor
    {
        public virtual IBlockProcessor BlockStepProcessor { get; set;} = new NullBlockHandler();
        public virtual ITransactionProcessor TransactionHandler { get; set;} = new NullTransactionHandler();
        public virtual ITransactionLogHandler TransactionLogHandler { get; set;} = new NullTransactionLogHandler();
        public virtual ITransactionVMStackHandler TransactionVmStackHandler { get; set;} = new NullTransactionVMStackHandler();
        public virtual IContractHandler ContractHandler { get; set;} = new NullContractHandler();
        
    }
     
     */

    //ProcessingStepMatch
    //MatchCriteria(Predicate, Async)
    //MatchCriteria(Predicate)
    //MatchCriteria(Async)
    //IsMatchAsync(T)
    //BlockStepMatch 
    //TransactionStepMatch
    //TransactionReceiptMatch

    //Processor (blockStepMatch 
    // Processing...
    // BlockProccesing predicateMatch (Step1) Date starting X and  Date end Y (BlockMatching)
    // TransactionProcessing predicateMatch (Step2) (TransactionMatching)  two types one examine the data in memory (Business rules loaded and standup data)
    // use external stuff as we cannot hold it in memory.
    // ReceiptProcessing predicateMatch (Step3) (ReceiptMatching)


    /*
     var filters = new FilterCollection()                .Add<Block>(b => b.Author == "Dave")                .Add<Transaction>(tx => tx.IsFrom("x"))   //             .Add<Transaction> tx => Task.FromResult(true)) /* simulate async db lookup */
    // .Add<TransactionWithReceipt>(tr => tr.TransactionReceipt.Succeeded())
    // .Add<TransactionLogWrapper>(l => l.Log.Removed == false);

    /*
     * var blockProcessorMatcher =  new BPM() with step handlers
     *   .SetBlockStepMatchCriteria(b => b.Author == "dave")
     *   .HandleBlockStepAsync(block => xx)
     *   .SetTransactionStepMatchCriteria(tx => tx.IsFrom("x) && tx.IsForCreation(), async (tx) => dbLookup.(tx) address) tx
     *   .SetTransactionReceiptMatchCriteria(receipt.ContractAddress == "xx")
     */


    public class CrawlerStepCompleted<T>
    {
        public CrawlerStepCompleted(IEnumerable<BlockchainProcessorExecutionSteps> executedStepsCollection, T stepData)
        {
            ExecutedStepsCollection = executedStepsCollection;
            StepData = stepData;
        }

        public IEnumerable<BlockchainProcessorExecutionSteps> ExecutedStepsCollection { get; private set; }
        public T StepData { get; private set; }

    }

    public abstract class CrawlerStep<TParentStep, TProcessStep>
    {
        protected IWeb3 Web3 { get; }

        public CrawlerStep(
            IWeb3 web3
        )
        {
            Web3 = web3;
        }

        public abstract Task<TProcessStep> GetStepDataAsync(TParentStep parentStep);

        public virtual async Task<CrawlerStepCompleted<TProcessStep>> ExecuteStepAsync(TParentStep parentStep, IEnumerable<BlockchainProcessorExecutionSteps> executionStepsCollection)
        {
            var processStepValue = await GetStepDataAsync(parentStep);
            if (processStepValue == null) return null;
            var stepsToProcesss =
                await executionStepsCollection.FilterMatchingStepAsync(parentStep).ConfigureAwait(false);

            if (stepsToProcesss.Any())
            {
                await stepsToProcesss.ExecuteCurrentStepAsync(parentStep);
            }
            return new CrawlerStepCompleted<TProcessStep>(stepsToProcesss, processStepValue);

        }
    }

    public class BlockCrawler3 : CrawlerStep<BigInteger, BlockWithTransactions>
    {
        public BlockCrawler3(IWeb3 web3) : base(web3)
        {

        }
        public override Task<BlockWithTransactions> GetStepDataAsync(BigInteger blockNumber)
        {
            return Web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(blockNumber.ToHexBigInteger());
        }
    }


    public class BlockCrawler2
    {
        private readonly IEnumerable<IProcessor<Block>> _blockStepProcessors;
        protected IEthApiContractService BlockProxy { get; }
        protected ITransactionProcessor TransactionProcessor { get; }

        private readonly UniqueTransactionHashList _processedTransactions = new UniqueTransactionHashList();
        private BigInteger _lastBlock;
        private readonly object _sync = new object();

        public BlockCrawler2(
            IWeb3 web3,
            IEnumerable<IProcessor<Block>> blockStepProcessors
            //ITransactionProcessor transactionProcessor,
        )
        {
            _blockStepProcessors = blockStepProcessors;
            BlockProxy = web3.Eth;
            //TransactionProcessor = transactionProcessor;
        }

        public bool ProcessTransactionsInParallel { get; set; } = true;

        public virtual async Task ProcessBlockAsync(BigInteger blockNumber)
        {
            var block = await BlockProxy
                .Blocks
                .GetBlockWithTransactionsByNumber.SendRequestAsync(blockNumber.ToHexBigInteger())
                .ConfigureAwait(false);

            if (block == null)
                throw new BlockNotFoundException(blockNumber);


            if (await _blockStepProcessors.IsStepMatchAsync(block))
            {
                await BlockHandler.HandleAsync(block);

                if (ProcessTransactionsInParallel)
                    await ProcessTransactionsMultiThreaded(block).ConfigureAwait(false);
                else
                    await ProcessTransactions(block).ConfigureAwait(false);
            }
        }

        public virtual async Task<BigInteger> GetMaxBlockNumberAsync()
        {
            var blockNumber = await BlockProxy.Blocks.GetBlockNumber.SendRequestAsync().ConfigureAwait(false);
            return blockNumber.Value;
        }

        protected virtual void ClearCacheOfProcessedTransactions()
        {
            lock (_sync)
            {
                _processedTransactions.Clear();
            }
        }

        protected virtual bool HasAlreadyBeenProcessed(Transaction transaction)
        {
            lock (_sync)
            {
                return _processedTransactions.Contains(transaction.TransactionHash);
            }
        }

        protected virtual void MarkAsProcessed(Transaction transaction)
        {
            lock (_sync)
            {
                _processedTransactions.Add(transaction.TransactionHash);
            }
        }

        protected virtual async Task ProcessTransactions(BlockWithTransactions block)
        {
            foreach (var txn in block.Transactions)
            {
                if (!HasAlreadyBeenProcessed(txn))
                {
                    await TransactionProcessor.ProcessTransactionAsync(block, txn)
                        .ConfigureAwait(false);
                    MarkAsProcessed(txn);
                }
            }
        }

        protected virtual async Task ProcessTransactionsMultiThreaded(BlockWithTransactions block)
        {
            var txTasks = new List<Task>(block.Transactions.Length);

            foreach (var txn in block.Transactions)
            {
                if (!HasAlreadyBeenProcessed(txn))
                {
                    var task = TransactionProcessor.ProcessTransactionAsync(block, txn)
                        .ContinueWith((t) =>
                        {
                            MarkAsProcessed(txn);
                        });

                    txTasks.Add(task);
                }
            }

            await Task.WhenAll(txTasks).ConfigureAwait(false);
        }
    }


    public class BlockCrawler : IBlockCrawler
    {
        protected IEthApiContractService BlockProxy { get; }
        protected IBlockHandler BlockHandler { get; }
        protected IEnumerable<IBlockFilter> BlockFilters { get; }
        protected ITransactionProcessor TransactionProcessor { get; }

        private readonly UniqueTransactionHashList _processedTransactions = new UniqueTransactionHashList();
        private BigInteger _lastBlock;
        private readonly object _sync = new object();

        public BlockCrawler(
            IWeb3 web3, 
            IBlockHandler blockHandler,
            ITransactionProcessor transactionProcessor, 
            IEnumerable<IBlockFilter> blockFilters = null
           )
        {
            BlockProxy = web3.Eth;
            BlockHandler = blockHandler;
            TransactionProcessor = transactionProcessor;
            BlockFilters = blockFilters ?? new IBlockFilter[0];
        }

        public bool ProcessTransactionsInParallel { get; set; } = true;

        public virtual async Task ProcessBlockAsync(BigInteger blockNumber)
        {
            if (_lastBlock != blockNumber)
            {
                ClearCacheOfProcessedTransactions();
                _lastBlock = blockNumber;
            }

            var block = await BlockProxy
                .Blocks
                .GetBlockWithTransactionsByNumber.SendRequestAsync(blockNumber.ToHexBigInteger())
                .ConfigureAwait(false);

            if(block == null)
                throw new BlockNotFoundException(blockNumber);

            if (await BlockFilters.IsMatchAsync(block))
            {
                await BlockHandler.HandleAsync(block);

                if (ProcessTransactionsInParallel)
                    await ProcessTransactionsMultiThreaded(block).ConfigureAwait(false);
                else
                    await ProcessTransactions(block).ConfigureAwait(false);
            }
        }

        public virtual async Task<BigInteger> GetMaxBlockNumberAsync()
        {
            var blockNumber = await BlockProxy.Blocks.GetBlockNumber.SendRequestAsync().ConfigureAwait(false);
            return blockNumber.Value;
        }

        protected virtual void ClearCacheOfProcessedTransactions()
        {
            lock (_sync)
            {
                _processedTransactions.Clear();
            }
        }
       
        protected virtual bool HasAlreadyBeenProcessed(Transaction transaction)
        {
            lock (_sync)
            {
                return _processedTransactions.Contains(transaction.TransactionHash);
            }
        }

        protected virtual void MarkAsProcessed(Transaction transaction)
        {
            lock (_sync)
            {
                _processedTransactions.Add(transaction.TransactionHash);
            }
        }

        protected virtual async Task ProcessTransactions(BlockWithTransactions block)
        {
            foreach (var txn in block.Transactions)
            {
                if (!HasAlreadyBeenProcessed(txn))
                {
                    await TransactionProcessor.ProcessTransactionAsync(block, txn)
                        .ConfigureAwait(false);
                    MarkAsProcessed(txn);
                }
            }
        }

        protected virtual async Task ProcessTransactionsMultiThreaded(BlockWithTransactions block)
        {
            var txTasks = new List<Task>(block.Transactions.Length);

            foreach (var txn in block.Transactions)
            {
                if (!HasAlreadyBeenProcessed(txn))
                {
                    var task = TransactionProcessor.ProcessTransactionAsync(block, txn)
                        .ContinueWith((t) =>
                    {
                        MarkAsProcessed(txn);
                    });

                    txTasks.Add(task);
                }
            }

            await Task.WhenAll(txTasks).ConfigureAwait(false);
        }
    }
}