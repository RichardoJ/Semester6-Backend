using CatalogNoSQL.Model;

namespace CatalogNoSQL.Service
{
    public interface IPaperService
    {
        Task<IEnumerable<Paper>> GetAllPapersAsync();
        Task<IEnumerable<Paper>> GetAllPapersByAuthorAsync(int authorId);
        Task<Paper> GetPaperByIdAsync(string id);

    }
}
