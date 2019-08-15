namespace Nethereum.LogProcessing.Dynamic.Handling.Handlers
{
    public interface IQueueMessageMapper
    {
        object Map(DecodedEvent decodedEvent);
    }

}
