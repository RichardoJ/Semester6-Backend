using Amazon.Runtime.Internal.Util;
using CatalogNoSQL.Model;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Security.Authentication;
using System.Text.Json;

namespace CatalogNoSQL.Repository
{
    public class PaperRepository : IPaperRepository
    {
        private IMongoCollection<Paper> _papersCollection;
        private readonly IDistributedCache _cache;
        private readonly DistributedCacheEntryOptions _cacheOptions;

        public PaperRepository(IOptions<PaperStoreDatabaseSettings> settings, IDistributedCache cache )
        {
            MongoClientSettings mongoSettings = MongoClientSettings.FromUrl(new MongoUrl(settings.Value.ConnectionString));
            mongoSettings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
            var mongoClient = new MongoClient(mongoSettings);

            var mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);

            _papersCollection = mongoDatabase.GetCollection<Paper>(settings.Value.PapersCollectionName);

            _cache = cache;
            _cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60) // Cache data for 60 seconds
            };
        }

        public async void AddPaper(Paper paper)
        {
            await _papersCollection.InsertOneAsync(paper);
            // Invalidate the cache when a new paper is added
            await _cache.RemoveAsync("all-papers");
        }

        public async Task<IEnumerable<Paper>> GetAllPapersAsync()
        {
            var cachedPapers = await _cache.GetStringAsync("all-papers");
            if (!string.IsNullOrEmpty(cachedPapers))
            {
                // Return cached data
                Console.WriteLine("From cache");
                return JsonSerializer.Deserialize<IEnumerable<Paper>>(cachedPapers);
            }
            else
            {
                // Get data from the database
                var papers = await _papersCollection.Find(_ => true).ToListAsync();
                Console.WriteLine("From DB");
                // Cache the data
                await _cache.SetStringAsync("all-papers", JsonSerializer.Serialize(papers), _cacheOptions);
                return papers;
            }
        }

        public async Task<IEnumerable<Paper>> GetAllPapersByAuthor(int authorId)
        {
            var cachedPapers = await _cache.GetStringAsync($"papers-by-author-{authorId}");
            if (!string.IsNullOrEmpty(cachedPapers))
            {
                // Return cached data
                return JsonSerializer.Deserialize<IEnumerable<Paper>>(cachedPapers);
            }
            else
            {
                // Get data from the database
                var papers = await _papersCollection.Find(x => x.AuthorId == authorId).ToListAsync();
                // Cache the data
                await _cache.SetStringAsync($"papers-by-author-{authorId}", JsonSerializer.Serialize(papers), _cacheOptions);
                return papers;
            }
        }

        public async Task<Paper> GetPaperAsync(string id)
        {
            var cachedPaper = await _cache.GetStringAsync($"paper-{id}");
            if (!string.IsNullOrEmpty(cachedPaper))
            {
                // Return cached data
                return JsonSerializer.Deserialize<Paper>(cachedPaper);
            }
            else
            {
                // Get data from the database
                var paper = await _papersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (paper != null)
                {
                    // Cache the data
                    await _cache.SetStringAsync($"paper-{id}", JsonSerializer.Serialize(paper), _cacheOptions);
                }
                return paper;
            }
        }

        public async void RemovePaper(string id)
        {
            await _papersCollection.DeleteOneAsync(x => x.Id == id);
            await _cache.RemoveAsync("all-papers");
        }

        public async void RemovePaperByAuthorId(int id)
        {
            await _papersCollection.DeleteManyAsync(x => x.AuthorId == id);
            await _cache.RemoveAsync("all-papers");
        }

        public async void UpdatePaper(Paper paper)
        {
            await _papersCollection.ReplaceOneAsync(x => x.Id == paper.Id, paper);
            await _cache.RemoveAsync("all-papers");
        }
    }
}
