using CatalogNoSQL.Model;

namespace CatalogNoSQL.Repository
{
    public interface IPaperRepository
    {
        void AddPaper(Paper paper);

        void UpdatePaper(Paper paper);

        void RemovePaper(string id);

        void RemovePaperByAuthorId(int id);

        Task<IEnumerable<Paper>> GetAllPapersAsync();

        Task<IEnumerable<Paper>> GetAllPapersByAuthor(int authorId);

        Task<Paper> GetPaperAsync(string id);
    }
}
