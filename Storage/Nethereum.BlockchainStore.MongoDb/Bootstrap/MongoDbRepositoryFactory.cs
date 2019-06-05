using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using Nethereum.BlockchainStore.MongoDb.Repositories;
using Nethereum.BlockchainStore.Repositories;

namespace Nethereum.BlockchainStore.MongoDb.Bootstrap
{
    public class MongoDbRepositoryFactory : IBlockchainStoreRepositoryFactory
    {
        public static MongoDbRepositoryFactory Create(IConfigurationRoot config, bool deleteAllExistingCollections = false)
        {
            var connectionString = config.GetMongoDbConnectionStringOrThrow();

            var factory = new MongoDbRepositoryFactory(connectionString);

            var db = factory.CreateDbIfNotExists();

            if (deleteAllExistingCollections)
                factory.DeleteAllCollections(db).Wait();

            factory.CreateCollectionsIfNotExist(db).Wait();

            return factory;
        }

        private readonly IMongoClient _client;
        private readonly string _databaseName;

        public MongoDbRepositoryFactory(string connectionString)
        {
            _databaseName = "BlockchainStorage";
            _client = new MongoClient(connectionString);
        }

        public IMongoDatabase CreateDbIfNotExists()
        {
            return _client.GetDatabase(_databaseName);
        }

        public async Task DeleteDatabase()
        {
            await _client.DropDatabaseAsync(_databaseName);
        }

        public async Task CreateCollectionsIfNotExist(IMongoDatabase db)
        {
            foreach (var collection in Enum.GetNames(typeof(MongoDbCollectionName)))
            {
                var collections = await db.ListCollectionsAsync(new ListCollectionsOptions
                    {Filter = new BsonDocument("name", collection)});

                if (await collections.AnyAsync())
                {
                    continue;
                }

                await db.CreateCollectionAsync(collection);
            }
        }

        public async Task DeleteAllCollections(IMongoDatabase db)
        {
            foreach (var collection in Enum.GetNames(typeof(MongoDbCollectionName)))
            {
                await db.DropCollectionAsync(collection);
            }
        }

        public IAddressTransactionRepository CreateAddressTransactionRepository() => new AddressTransactionRepository(_client, _databaseName);
        public IBlockRepository CreateBlockRepository() => new BlockRepository(_client, _databaseName);
        public IContractRepository CreateContractRepository() => new ContractRepository(_client, _databaseName);
        public ITransactionLogRepository CreateTransactionLogRepository() => new TransactionLogRepository(_client, _databaseName);
        public ITransactionRepository CreateTransactionRepository() => new TransactionRepository(_client, _databaseName);
        public ITransactionVMStackRepository CreateTransactionVmStackRepository() => new TransactionVMStackRepository(_client, _databaseName);
    }
}