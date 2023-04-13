namespace PublishNoSQL.EventProcessor
{
    public interface IEventProcessor
    {
        void ProcessEvent(string message);
    }
}
