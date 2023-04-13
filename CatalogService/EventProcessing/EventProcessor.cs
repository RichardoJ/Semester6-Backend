using AutoMapper;
using CatalogService.Dto;
using CatalogService.Model;
using CatalogService.Repository;
using CatalogService.Service;
using System.Text.Json;

namespace CatalogService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
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
            using(var scoper = _scopeFactory.CreateScope())
            {
                var repo = scoper.ServiceProvider.GetRequiredService<IPaperRepository>();

                var repoCategory = scoper.ServiceProvider.GetRequiredService<ICategoryRepo>();

                var publishedPaperDto = JsonSerializer.Deserialize<PaperPublishedDto>(platformPublishedMessage);

                try
                {
                    var paperCategory = publishedPaperDto.category;
                    Category category = repoCategory.GetCategory(paperCategory);
                    Console.WriteLine(category.Name);
                    Paper newPaper = new Paper();
                    newPaper.Title = publishedPaperDto.Title;
                    newPaper.Description = publishedPaperDto.Description;
                    newPaper.Author = publishedPaperDto.Author;
                    newPaper.PublishedYear = publishedPaperDto.PublishedYear;
                    newPaper.Category = category;
                    newPaper.AuthorId = publishedPaperDto.AuthorId;
                    newPaper.CiteCount = publishedPaperDto.CiteCount;
                    newPaper.PaperLink = publishedPaperDto.PaperLink;
                    if(repo.PaperExist(newPaper.Title) == false)
                    {
                        repo.AddPaper(newPaper);
                        repo.SaveChanges();
                        Console.WriteLine("--> Paper added");
                    }
                    else
                    {
                        Console.WriteLine("--> Same Title Detected");
                    }
                    
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

                var repoCategory = scoper.ServiceProvider.GetRequiredService<ICategoryRepo>();

                var updatedPaperDto = JsonSerializer.Deserialize<PaperUpdatedDto>(platformPublishedMessage);

                try
                {
                    var paperCategory = updatedPaperDto.category;
                    Category category = repoCategory.GetCategory(paperCategory);
                    Console.WriteLine(category.Name);
                    Paper newPaper = new Paper();
                    newPaper.Id = updatedPaperDto.Id;
                    newPaper.Title = updatedPaperDto.Title;
                    newPaper.Description = updatedPaperDto.Description;
                    newPaper.Author = updatedPaperDto.Author;
                    newPaper.PublishedYear = updatedPaperDto.PublishedYear;
                    newPaper.Category = category;
                    newPaper.AuthorId = updatedPaperDto.AuthorId;
                    newPaper.CiteCount = updatedPaperDto.CiteCount;
                    newPaper.PaperLink = updatedPaperDto.PaperLink;
                    repo.UpdatePaper(newPaper);
                    repo.SaveChanges();
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
            using(var scoper = _scopeFactory.CreateScope())
            {
                var repo = scoper.ServiceProvider.GetRequiredService<IPaperRepository>();

                var deletedUserDto = JsonSerializer.Deserialize<UserDeleteDto>(userDeletedMessage);

                try
                {
                    repo.RemovePaperByAuthorId(deletedUserDto.Id);
                    repo.SaveChanges();
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
                    repo.SaveChanges();
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
