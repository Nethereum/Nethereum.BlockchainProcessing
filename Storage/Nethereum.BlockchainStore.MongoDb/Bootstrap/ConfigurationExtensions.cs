﻿using Microsoft.Extensions.Configuration;
using Nethereum.Configuration;

namespace Nethereum.BlockchainStore.MongoDb.Bootstrap
{
    public static class ConfigurationExtensions
    {
        public static void SetMongoDbConnectionString(this IConfigurationRoot appConfig, string connectionString)
        {
            appConfig[ConfigurationKeyNames.MongoDbConnectionString] = connectionString;
        }

        public static string GetMongoDbConnectionStringOrThrow(this IConfigurationRoot config)
        {
            return config.GetOrThrow(ConfigurationKeyNames.MongoDbConnectionString);
        }
    }
}