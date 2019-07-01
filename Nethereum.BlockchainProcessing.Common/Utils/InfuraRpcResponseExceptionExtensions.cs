using System;
using Nethereum.JsonRpc.Client;

namespace Nethereum.BlockchainProcessing
{
    public static class InfuraRpcResponseExceptionExtensions
    {
        public static readonly string TooManyRecordsMessagePrefix = "query returned more than";

        public static bool IsInfuraTooManyRecords(this RpcResponseException rpcResponseEx)
        {
            return rpcResponseEx.Message.StartsWith(TooManyRecordsMessagePrefix);
        }

        public static InfuraTooManyRecordsException CreateInfuraTooManyRecordsException(this RpcResponseException rpcResponseEx)
        {
            return new InfuraTooManyRecordsException(rpcResponseEx);
        }
    }
}
