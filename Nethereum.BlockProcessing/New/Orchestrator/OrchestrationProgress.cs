using System;
using System.Numerics;

namespace Nethereum.BlockchainProcessing.Processors
{
    public class OrchestrationProgress
    {
        public BigInteger? BlockNumberProcessTo { get; set; }
        public Exception Exception { get; set; }
        public bool HasErrored => Exception != null;
    }
}