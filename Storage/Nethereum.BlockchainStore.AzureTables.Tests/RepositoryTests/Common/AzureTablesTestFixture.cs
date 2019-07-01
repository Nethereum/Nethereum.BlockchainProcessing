using Nethereum.BlockchainStore.AzureTables.Bootstrap;
using System;
using Microsoft.Configuration.Utils;
using Xunit;

namespace Nethereum.BlockchainStore.AzureTables.Tests.RepositoryTests
{
    public class AzureTablesFixture : IDisposable
    {
        public static readonly string[] CommandLineArgs = new string[] {};
        public static readonly string UserSecretsId = "Nethereum.BlockchainStore.AzureTables";

        public AzureTablesFixture()
        {
            ConfigurationUtils.SetEnvironmentAsDevelopment();
            var appConfig = ConfigurationUtils.Build(CommandLineArgs, UserSecretsId);
            var connectionString = appConfig["AzureStorageConnectionString"];
            BlockProcessingCloudTableSetup = new BlockProcessingCloudTableSetup(connectionString, "UnitTest");
            BlockProgressCloudTableSetup = new BlockProgressCloudTableSetup(connectionString, "UnitTest");
        }

        public BlockProcessingCloudTableSetup BlockProcessingCloudTableSetup { get; }
        public BlockProgressCloudTableSetup BlockProgressCloudTableSetup { get; }

        public void Dispose()
        {
            BlockProcessingCloudTableSetup?.DeleteAllTables().Wait();
            BlockProgressCloudTableSetup?.DeleteAllTables().Wait();
        }
    }

    [CollectionDefinition("AzureTablesFixture")]
    public class AzureTablesFixtureCollection : ICollectionFixture<AzureTablesFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
