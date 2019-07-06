using Nethereum.BlockchainProcessing.Common.Processing;
using Nethereum.BlockchainStore.Repositories.Handlers;

namespace Nethereum.BlockchainStore.Repositories
{
    public class StorageBlockchainProcessorExecutionSteps: BlockchainProcessorExecutionSteps
    {
        public IBlockRepository BlockRepository { get; }
        public IContractRepository ContractRepository { get; }
        public ITransactionRepository TransactionRepository { get; }
        public ITransactionLogRepository TransactionLogRepository { get; }
        public ITransactionVMStackRepository TransactionVmStackRepository { get; }
        public IAddressTransactionRepository AddressTransactionRepository { get; }

        public StorageBlockchainProcessorExecutionSteps(IBlockchainStoreRepositoryFactory repositoryFactory)
        {
            BlockRepository = repositoryFactory.CreateBlockRepository();
            ContractRepository = repositoryFactory.CreateContractRepository();
            TransactionRepository = repositoryFactory.CreateTransactionRepository();
            AddressTransactionRepository = repositoryFactory.CreateAddressTransactionRepository();
            TransactionLogRepository = repositoryFactory.CreateTransactionLogRepository();
            TransactionVmStackRepository = repositoryFactory.CreateTransactionVmStackRepository();
        }

        protected virtual void AddBlockStepStorageHandler(IBlockchainStoreRepositoryFactory repositoryFactory)
        {
            var handler = new BlockStepStorageHandler(repositoryFactory.CreateBlockRepository());
            this.BlockStepProcessor.AddProcessorHandler(handler);
        }

        protected virtual void AddContractCreationStepStorageHandler(IBlockchainStoreRepositoryFactory repositoryFactory)
        {
            var handler = new ContractCreationStorageStepHandler(repositoryFactory.CreateContractRepository());
            this.ContractCreationStepProcessor.AddProcessorHandler(handler);
        }

        protected virtual void AddTransactionStepStorageHandler(IBlockchainStoreRepositoryFactory repositoryFactory)
        {
            //var handler = new BlockRepositoryHandler2(repositoryFactory());
            //this.BlockStepProcessor.AddProcessorHandler(handler);
        }
    }
}
