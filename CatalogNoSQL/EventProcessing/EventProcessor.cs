using CatalogNoSQL.Dto;
using CatalogNoSQL.Model;
using CatalogNoSQL.Repository;
using System.Text.Json;

namespace CatalogNoSQL.EventProcessing
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
                case EventType.PaperPublished:
                    addPaper(message);
                    break;
                case EventType.PaperDeleted:
                    deletePaper(message);
                    break;
                case EventType.UserDeleted:
                    deleteUser(message);
                    break;
                case EventType.PaperUpdated:
                    updatePaper(message);
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
                case "Publish":
                    Console.WriteLine("Paper Published Event Detected");
                    return EventType.PaperPublished;
                case "Delete_paper":
                    Console.WriteLine("Paper Deleted Event Detected");
                    return EventType.PaperDeleted;
                case "Delete_user":
                    Console.WriteLine("User Deleted Event Detected");
                    return EventType.UserDeleted;
                case "Update_paper":
                    Console.WriteLine("Paper Updated Event Detected");
                    return EventType.PaperUpdated;
                default:
                    Console.WriteLine("--> Could not determined the event type");
                    return EventType.Undetermined;
            }
        }

        private void addPaper(string platformPublishedMessage)
        {
            using (var scoper = _scopeFactory.CreateScope())
            {
                var repo = scoper.ServiceProvider.GetRequiredService<IPaperRepository>();

                var publishedPaperDto = JsonSerializer.Deserialize<PaperPublishedDto>(platformPublishedMessage);

                try
                {

                    Paper newPaper = new Paper();
                    newPaper.Id = publishedPaperDto.Id;
                    newPaper.Title = publishedPaperDto.Title;
                    newPaper.Description = publishedPaperDto.Description;
                    newPaper.Author = publishedPaperDto.Author;
                    newPaper.PublishedYear = publishedPaperDto.PublishedYear;
                    newPaper.Category = publishedPaperDto.Category;
                    newPaper.AuthorId = publishedPaperDto.AuthorId;
                    newPaper.CiteCount = publishedPaperDto.CiteCount;
                    newPaper.PaperLink = publishedPaperDto.PaperLink;
                    //if (repo.PaperExist(newPaper.Title) == false)
                    //{

                    //}
                    //else
                    //{
                    //    Console.WriteLine("--> Same Title Detected");
                    //}
                    repo.AddPaper(newPaper);
                    Console.WriteLine("--> Paper added");

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Could not add Paper to DB {ex.Message}");
                    throw;
                }
            }
        }

        private void updatePaper(string platformPublishedMessage)
        {
            using (var scoper = _scopeFactory.CreateScope())
            {
                var repo = scoper.ServiceProvider.GetRequiredService<IPaperRepository>();

                var updatedPaperDto = JsonSerializer.Deserialize<PaperUpdatedDto>(platformPublishedMessage);

                try
                {
                    Paper newPaper = new Paper();
                    newPaper.Id = updatedPaperDto.Id;
                    newPaper.Title = updatedPaperDto.Title;
                    newPaper.Description = updatedPaperDto.Description;
                    newPaper.Author = updatedPaperDto.Author;
                    newPaper.PublishedYear = updatedPaperDto.PublishedYear;
                    newPaper.Category = updatedPaperDto.category;
                    newPaper.AuthorId = updatedPaperDto.AuthorId;
                    newPaper.CiteCount = updatedPaperDto.CiteCount;
                    newPaper.PaperLink = updatedPaperDto.PaperLink;
                    repo.UpdatePaper(newPaper);
                    Console.WriteLine("--> Paper Updated");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Could not Update Paper in DB {ex.Message}");
                    throw;
                }
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

        private void deletePaper(string platformDeletedMessage)
        {
            using (var scoper = _scopeFactory.CreateScope())
            {
                var repo = scoper.ServiceProvider.GetRequiredService<IPaperRepository>();

                var deletedPaperDto = JsonSerializer.Deserialize<PaperDeletedDto>(platformDeletedMessage);

                try
                {
                    repo.RemovePaper(deletedPaperDto.Id);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Could not delete Paper from DB {ex.Message}");
                    throw;
                }
            }
        }
    }

    enum EventType
    {
        PaperPublished,
        PaperDeleted,
        PaperUpdated,
        UserDeleted,
        Undetermined
    }
}
