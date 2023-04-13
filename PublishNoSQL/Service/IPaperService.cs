using PublishNoSQL.Model;

namespace PublishNoSQL.Service
{
    public interface IPaperService
    {
        Task AddPaperAsync(Paper paper);
        Task<Boolean> UpdatePaperAsync(Paper paper);
        Task<Boolean> RemovePaperAsync(string id);
        Task RemovePapersByAuthorIdAsync(int authorId);

        Task<string> ReturnPaperLink (string id);
    }
}
