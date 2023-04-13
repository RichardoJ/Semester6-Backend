using CatalogService.Model;

namespace CatalogService.Service
{
    public interface IPaperService
    {

        IEnumerable<Paper> GetAllPapers();

        IEnumerable<Paper> GetAllPapersByAuthor(int authorId);

        IEnumerable<Paper> GetAllPapersPagination(Parameters parameters);

        Paper GetPaperById(int id);

        bool PaperExist(string name);
    }
}
