using PublishNoSQL.Model;
using PublishNoSQL.Repository;
using System.Web;

namespace PublishNoSQL.Service
{
    public class PaperService : IPaperService
    {
        private readonly IPaperRepository _paperRepository;

        public PaperService(IPaperRepository paperRepository)
        {
            _paperRepository = paperRepository;
        }

        public Task AddPaperAsync(Paper paper)
        {
            _paperRepository.AddPaper(paper);
            return Task.CompletedTask;
        }

        public async Task<Boolean> UpdatePaperAsync(Paper paper)
        {
            var findPaper = await _paperRepository.GetPaperAsync(paper.Id);
            if(findPaper != null)
            {
                _paperRepository.UpdatePaper(paper);
                return true;
            }
            else
            {
                return false;
            }
            
        }

        public async Task<Boolean> RemovePaperAsync(string id)
        {
            var findPaper = await _paperRepository.GetPaperAsync(id);

            if (findPaper != null)
            {
                _paperRepository.RemovePaper(id);
                return true;
            }
            else
            {
                return false;
            }

            
        }

        public Task RemovePapersByAuthorIdAsync(int authorId)
        {
            _paperRepository.RemovePaperByAuthorId(authorId);
            return Task.CompletedTask;
        }

        public async Task<string> ReturnPaperLink(string id)
        {
            var findPaper = await _paperRepository.GetPaperAsync(id);
            var url = findPaper.PaperLink;
            string fileName = Path.GetFileName(url);
            string decodedFileName = HttpUtility.UrlDecode(fileName);
            if (findPaper != null)
            {
                return decodedFileName;
            }
            else
            {
                return "";
            }
        }
    }
}
