using Microsoft.WindowsAzure.Storage.Table;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.LogProcessing.Dynamic.Configuration;
using System;

namespace Nethereum.LogProcessing.Dynamic.Db.Azure.Entities
{
    public class ParameterConditionEntity : TableEntity, IParameterConditionDto
    {
        public long Id
        {
            get => this.RowKeyToLong();
            set => RowKey = value.ToString();
        }

        public long EventSubscriptionId
        {
            get => this.PartionKeyToLong();
            set => PartitionKey = value.ToString();
        }
        public int ParameterOrder {get;set; }

        public string OperatorAsString
        {
            get {  return Operator.ToString();}
            set {  Operator = (ParameterConditionOperator)Enum.Parse(typeof(ParameterConditionOperator), value); }
        }
        public ParameterConditionOperator Operator { get; set; }
        public string Value { get; set; }

    }
}
