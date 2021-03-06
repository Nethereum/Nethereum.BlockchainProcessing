﻿using Nethereum.BlockchainProcessing.Processor;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

public class BlockProcessing_WithTransactionCriteria
{
    /// <summary>
    /// Process transactions matching specific criteria
    /// </summary>
    /// <returns></returns>
    public async Task WithTransactionCriteria()
    {
        var transactions = new List<TransactionReceiptVO>();
        var filterLogs = new List<FilterLogVO>();

        var web3 = new Web3("https://rinkeby.infura.io/v3/7238211010344719ad14a89db874158c");

        //create our processor
        var processor = web3.Processing.Blocks.CreateBlockProcessor(steps => {
            steps.TransactionStep.SetMatchCriteria(t => t.Transaction.IsFrom("0x1cbff6551b8713296b0604705b1a3b76d238ae14"));
            steps.TransactionReceiptStep.AddSynchronousProcessorHandler(tx => transactions.Add(tx));
            steps.FilterLogStep.AddSynchronousProcessorHandler(l => filterLogs.Add(l));
        });

        //if we need to stop the processor mid execution - call cancel on the token
        var cancellationToken = new CancellationToken();
        //crawl the blocks
        await processor.ExecuteAsync(
            toBlockNumber: new BigInteger(2830145),
            cancellationToken: cancellationToken,
            startAtBlockNumberIfNotProcessed: new BigInteger(2830144));

        Console.WriteLine($"Transactions. Expected: 2, Actual: {transactions.Count}");
        Console.WriteLine($"Logs. Expected: 4, Actual: {filterLogs.Count}");
    }

}


