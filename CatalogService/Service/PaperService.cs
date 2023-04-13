using CatalogService.Model;
using CatalogService.Repository;

namespace CatalogService.Service
{
    public class PaperService : IPaperService
    {
        private readonly IPaperRepository _repository;

        public PaperService(IPaperRepository repository)
        {
            _repository = repository;
        }

        public Paper GetPaperById(int id)
        {
            return _repository.GetPaper(id);
        }

        public IEnumerable<Paper> GetAllPapers()
        {
            return _repository.GetAllPapers();
        }

        public IEnumerable<Paper> GetAllPapersPagination(Parameters parameters)
        {
            return _repository.GetAllPapersPagination(parameters);
        }

        public bool PaperExist(string name)
        {
            return _repository.PaperExist(name);
        }

        public IEnumerable<Paper> GetAllPapersByAuthor(int authorId)
        {
            return _repository.GetAllPapersByAuthor(authorId);
        }
    }
}
