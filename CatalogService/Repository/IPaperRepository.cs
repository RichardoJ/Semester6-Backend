using CatalogService.Model;

namespace CatalogService.Repository
{
    public interface IPaperRepository
    {
        bool SaveChanges();

        void AddPaper(Paper paper);

        void UpdatePaper(Paper paper);

        void RemovePaper(int id);

        void RemovePaperByAuthorId(int id);

        IEnumerable<Paper> GetAllPapers();

        IEnumerable<Paper> GetAllPapersByAuthor(int authorId);

        IEnumerable<Paper> GetAllPapersPagination(Parameters parameters);

        Paper GetPaper(int id);

        bool PaperExist(string name);
    }
}
