using PublishService.Model;
using PublishService.Repository;

namespace PublishService.Service
{
    public class PaperService : IPaperService
    {
        private readonly IPaperRepository _repository;

        public PaperService(IPaperRepository repository)
        {
            _repository = repository;
        }

        public Boolean AddPaper(Paper paper)
        {
            if(_repository.GetPaperByTitle(paper.Title) == false) {
                _repository.AddPaper(paper);
                _repository.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean RemovePaper(int id)
        {
            if(_repository.GetPaper(id) != null)
            {
                _repository.RemovePaper(id);
                _repository.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean UpdatePaper(Paper paper)
        {
            var result = _repository.GetPaper(paper.Id);
            if (result != null)
            {
                _repository.UpdatePaper(paper);
                _repository.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
