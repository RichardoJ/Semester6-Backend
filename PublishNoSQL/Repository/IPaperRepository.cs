using PublishNoSQL.Model;

namespace PublishNoSQL.Repository
{
    public interface IPaperRepository
    {
        void AddPaper(Paper paper);

        void UpdatePaper(Paper paper);

        void RemovePaper(string id);

        void RemovePaperByAuthorId(int id);

        Task<Paper> GetPaperAsync(string id);
    }
}
