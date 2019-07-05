using System.Threading.Tasks;
using Nethereum.BlockchainProcessing.Common.Processing;
using Nethereum.BlockProcessing.ValueObjects;

namespace Nethereum.BlockchainStore.Repositories.Handlers
{
    public class ContractCreationStorageStepHandler : IProcessorHandler<ContractCreationTransaction>
    {
        private readonly IContractRepository _contractRepository;
        public ContractCreationStorageStepHandler(IContractRepository contractRepository)
        {
            _contractRepository = contractRepository;
        }
        public Task ExecuteAsync(ContractCreationTransaction contractCreation)
        {
            return _contractRepository.UpsertAsync(
               contractCreation.ContractAddress,
               contractCreation.Code,
               contractCreation.Transaction);
        }
    }
}
