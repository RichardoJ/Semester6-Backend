using PublishService.Model;

namespace PublishService.Repository
{
    public interface IPaperRepository
    {
        bool SaveChanges();

        void AddPaper(Paper paper);

        void UpdatePaper(Paper paper);

        void RemovePaper(int id);

        void RemovePaperByAuthorId(int id);

        IEnumerable<Paper> GetAllPapers();

        Boolean GetPaperByTitle(string title);

        Paper GetPaper(int id);
    }
}
