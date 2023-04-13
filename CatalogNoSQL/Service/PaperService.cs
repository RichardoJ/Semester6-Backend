using CatalogNoSQL.Model;
using CatalogNoSQL.Repository;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ZstdSharp.Unsafe;

namespace CatalogNoSQL.Service
{
    public class PaperService : IPaperService
    {
        private readonly IPaperRepository _paperRepository;

        public PaperService(IPaperRepository paperRepository)
        {
            _paperRepository = paperRepository;
        }

        public async Task<IEnumerable<Paper>> GetAllPapersAsync()
        {
            return await _paperRepository.GetAllPapersAsync();
        }

        public async Task<IEnumerable<Paper>> GetAllPapersByAuthorAsync(int authorId)
        {
            return await _paperRepository.GetAllPapersByAuthor(authorId);
        }

        public async Task<Paper> GetPaperByIdAsync(string id)
        {
            return await _paperRepository.GetPaperAsync(id);
        }
    }
}
