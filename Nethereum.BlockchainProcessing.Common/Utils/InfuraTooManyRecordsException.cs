using System;
using Nethereum.JsonRpc.Client;

namespace Nethereum.BlockchainProcessing
{
    public class InfuraTooManyRecordsException: Exception
    {
        public InfuraTooManyRecordsException()
        {

        }

        public InfuraTooManyRecordsException(RpcResponseException innerException):base(innerException.Message, innerException)
        {

        }
    }
}
