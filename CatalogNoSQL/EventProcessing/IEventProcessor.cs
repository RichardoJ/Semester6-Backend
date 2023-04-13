namespace CatalogNoSQL.EventProcessing
{
    public interface IEventProcessor
    {
        void ProcessEvent(string message);
    }
}
