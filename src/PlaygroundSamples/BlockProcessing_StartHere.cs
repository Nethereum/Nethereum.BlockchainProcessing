﻿using Nethereum.BlockchainProcessing.Processor;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

public class BlockProcessing_StartHere
{
    /// <summary>
    /// Crawl the chain for a block range and injest the data
    /// </summary>
    public static async Task Main(string[] args)
    {
        var blocks = new List<BlockWithTransactions>();
        var transactions = new List<TransactionReceiptVO>();
        var contractCreations = new List<ContractCreationVO>();
        var filterLogs = new List<FilterLogVO>();

        var web3 = new Web3("https://rinkeby.infura.io/v3/7238211010344719ad14a89db874158c");

        //create our processor
        var processor = web3.Processing.Blocks.CreateBlockProcessor(steps =>
        {
            // inject handler for each step
            steps.BlockStep.AddSynchronousProcessorHandler(b => blocks.Add(b));
            steps.TransactionReceiptStep.AddSynchronousProcessorHandler(tx => transactions.Add(tx));
            steps.ContractCreationStep.AddSynchronousProcessorHandler(cc => contractCreations.Add(cc));
            steps.FilterLogStep.AddSynchronousProcessorHandler(l => filterLogs.Add(l));
        });

        //if we need to stop the processor mid execution - call cancel on the token
        var cancellationToken = new CancellationToken();

        //crawl the required block range
        await processor.ExecuteAsync(
            toBlockNumber: new BigInteger(2830145),
            cancellationToken: cancellationToken,
            startAtBlockNumberIfNotProcessed: new BigInteger(2830144));

        Console.WriteLine($"Blocks.  Expected: 2, Found: {blocks.Count}");
        Console.WriteLine($"Transactions.  Expected: 25, Found: {transactions.Count}");
        Console.WriteLine($"Contract Creations.  Expected: 5, Found: {contractCreations.Count}");

        Log(transactions, contractCreations, filterLogs);
    }

    private static void Log(
        List<TransactionReceiptVO> transactions, 
        List<ContractCreationVO> contractCreations, 
        List<FilterLogVO> filterLogs)
    {
        Console.WriteLine("Sent From");
        foreach (var fromAddressGrouping in transactions.GroupBy(t => t.Transaction.From).OrderByDescending(g => g.Count()))
        {
            var logs = filterLogs.Where(l => fromAddressGrouping.Any((a) => l.Transaction.TransactionHash == a.TransactionHash));

            Console.WriteLine($"From: {fromAddressGrouping.Key}, Tx Count: {fromAddressGrouping.Count()}, Logs: {logs.Count()}");
        }

        Console.WriteLine("Sent To");
        foreach (var toAddress in transactions
            .Where(t => !t.Transaction.IsToAnEmptyAddress())
            .GroupBy(t => t.Transaction.To)
            .OrderByDescending(g => g.Count()))
        {
            var logs = filterLogs.Where(l => toAddress.Any((a) => l.Transaction.TransactionHash == a.TransactionHash));

            Console.WriteLine($"To: {toAddress.Key}, Tx Count: {toAddress.Count()}, Logs: {logs.Count()}");
        }

        Console.WriteLine("Contracts Created");
        foreach (var contractCreated in contractCreations)
        {
            var tx = transactions.Count(t => t.Transaction.IsTo(contractCreated.ContractAddress));
            var logs = filterLogs.Count(l => transactions.Any(t => l.Transaction.TransactionHash == t.TransactionHash));

            Console.WriteLine($"From: {contractCreated.ContractAddress}, Tx Count: {tx}, Logs: {logs}");
        }
    }
}


