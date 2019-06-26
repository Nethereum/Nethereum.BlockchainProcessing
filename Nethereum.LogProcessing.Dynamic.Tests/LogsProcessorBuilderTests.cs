using Moq;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.RPC.Eth.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.LogProcessing.Tests
{
    public class LogsProcessorBuilderTests
    {
        const string BLOCKCHAIN_URL = "http://localhost:8545/";

        [Fact]
        public void Add_Passing_EventSubscription()
        {
            var transferSubscription = new EventSubscription<TestData.Contracts.StandardContract.TransferEvent>();
            var processor = new LogsProcessorBuilder(blockchainUrl: BLOCKCHAIN_URL)
                .Add(transferSubscription);
            Assert.Same(transferSubscription, processor.Processors.First());
        }

        public class QueueMessage
        {
            public object Content { get; set; }
        }

        [Fact]
        public async Task AddAndQueue_ForFilterLog_CreatesExpectedProcessor()
        {
            var queuedMessages = new List<object>();
            var mockQueue = new Mock<IQueue>();

            mockQueue.Setup(q => q.AddMessageAsync(It.IsAny<object>()))
                .Callback<object>(msg => queuedMessages.Add(msg))
                .Returns(Task.CompletedTask);

            //set up with a predicate that always returns false
            var processor = new LogsProcessorBuilder(blockchainUrl: BLOCKCHAIN_URL)
                .AddToQueue(mockQueue.Object);

            var logProcessor = processor.Processors[0];

            var logsToProcess = new[] { new FilterLog() };
            await logProcessor.ProcessLogsAsync(logsToProcess);

            Assert.Single(queuedMessages);
            Assert.Same(logsToProcess[0], queuedMessages[0]);
        }

        [Fact]
        public async Task AddAndQueue_ForFilterLog_WithPredicate_CreatesExpectedProcessor()
        {
            var queuedMessages = new List<object>();
            var mockQueue = new Mock<IQueue>();

            mockQueue.Setup(q => q.AddMessageAsync(It.IsAny<object>()))
                .Callback<object>(msg => queuedMessages.Add(msg))
                .Returns(Task.CompletedTask);

            //set up with a predicate that always returns false
            var processor = new LogsProcessorBuilder(blockchainUrl: BLOCKCHAIN_URL)
                .AddToQueue(mockQueue.Object,
                predicate: (logs) => false);

            var logProcessor = processor.Processors[0];

            var logsToProcess = new[] { new FilterLog() };
            await logProcessor.ProcessLogsAsync(logsToProcess);

            Assert.Empty(queuedMessages);
        }

        [Fact]
        public async Task AddAndQueue_ForFilterLog_WithMapper_CreatesExpectedProcessor()
        {
            var queuedMessages = new List<object>();
            var mockQueue = new Mock<IQueue>();

            mockQueue.Setup(q => q.AddMessageAsync(It.IsAny<object>()))
                .Callback<object>(msg => queuedMessages.Add(msg))
                .Returns(Task.CompletedTask);

            var processor = new LogsProcessorBuilder(blockchainUrl: BLOCKCHAIN_URL)
                .AddToQueue(mockQueue.Object,
                mapper: (log) => new QueueMessage { Content = log });

            var logProcessor = processor.Processors[0];

            var logsToProcess = new[] { new FilterLog() };
            await logProcessor.ProcessLogsAsync(logsToProcess);

            Assert.IsType<QueueMessage>(queuedMessages.First());
        }

    }
}
