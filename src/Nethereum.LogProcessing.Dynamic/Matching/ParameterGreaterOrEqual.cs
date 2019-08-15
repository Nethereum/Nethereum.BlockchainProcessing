using System.Numerics;

namespace Nethereum.LogProcessing.Dynamic.Matching
{
    public class ParameterGreaterOrEqual : BigIntegerParameterConditionBase, IParameterCondition
    {
        public ParameterGreaterOrEqual(int parameterOrder, string val):base(parameterOrder, val){ }

        protected override bool IsTrue(BigInteger value) => value >= Value;
    }
}
