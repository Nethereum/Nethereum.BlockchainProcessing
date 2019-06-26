﻿using System.Threading.Tasks;
using Nethereum.BlockProcessing.ValueObjects;

namespace Nethereum.BlockchainProcessing.Handlers
{
    public class NullTransactionLogHandler : ITransactionLogHandler
    {
        public Task HandleAsync(TransactionLogWrapper transactionLog)
        {
            return Task.CompletedTask;
        }
    }
}