using PublishNoSQL.Dto;
using PublishNoSQL.Repository;
using System.Text.Json;

namespace PublishNoSQL.EventProcessor
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public EventProcessor(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);

            switch (eventType)
            {
                case EventType.UserDeleted:
                    deleteUser(message);
                    break;
                case EventType.Undetermined:
                    break;
            }
        }

        private EventType DetermineEvent(string notificationMessage)
        {
            Console.WriteLine("--> Determining Event");

            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

            switch (eventType.Event)
            {
                case "Delete_user":
                    Console.WriteLine("User Deleted Event Detected");
                    return EventType.UserDeleted;
                default:
                    Console.WriteLine("--> Could not determined the event type");
                    return EventType.Undetermined;
            }
        }

        private void deleteUser(string userDeletedMessage)
        {
            using (var scoper = _scopeFactory.CreateScope())
            {
                var repo = scoper.ServiceProvider.GetRequiredService<IPaperRepository>();

                var deletedUserDto = JsonSerializer.Deserialize<UserDeletedDto>(userDeletedMessage);

                try
                {
                    repo.RemovePaperByAuthorId(deletedUserDto.Id);
                    Console.WriteLine($"--> All Paper with Author ID = {deletedUserDto.Id} have been deleted");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Could not delete Paper with Author id from DB {ex.Message}");
                    throw;
                }
            }
        }

        enum EventType
        {
            UserDeleted,
            Undetermined
        }
    }
}
