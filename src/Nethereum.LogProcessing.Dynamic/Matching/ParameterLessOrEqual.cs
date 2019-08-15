using System.Numerics;

namespace Nethereum.LogProcessing.Dynamic.Matching
{
    public class ParameterLessOrEqual : BigIntegerParameterConditionBase, IParameterCondition
    {
        public ParameterLessOrEqual(int parameterOrder, string val):base(parameterOrder, val){ }

        protected override bool IsTrue(BigInteger value) => value <= Value;
    }
}
