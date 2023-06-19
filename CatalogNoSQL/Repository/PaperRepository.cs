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
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(120) // Cache data for 60 seconds
            };
        }

        public async void AddPaper(Paper paper)
        {
            try
            {
                await _papersCollection.InsertOneAsync(paper);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding paper to MongoDB: {ex.Message}");
            }

            try
            {
                // Invalidate the cache when a new paper is added
                await _cache.RemoveAsync("all-papers");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing all-papers from cache: {ex.Message}");
            }
        }

        public async Task<IEnumerable<Paper>> GetAllPapersAsync()
        {
            try
            {
                var cachedPapers = await _cache.GetStringAsync("all-papers");
                if (!string.IsNullOrEmpty(cachedPapers))
                {
                    // Return cached data
                    Console.WriteLine("From cache");
                    return JsonSerializer.Deserialize<IEnumerable<Paper>>(cachedPapers);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting data from cache: {ex.Message}");
            }

            // Get data from the database
            var papers = await _papersCollection.Find(_ => true).ToListAsync();
            Console.WriteLine("From DB");

            try
            {
                // Cache the data
                await _cache.SetStringAsync("all-papers", JsonSerializer.Serialize(papers), _cacheOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error caching data: {ex.Message}");
            }

            return papers;
        }


        public async Task<IEnumerable<Paper>> GetAllPapersByAuthor(int authorId)
        {
            try
            {
                var cachedPapers = await _cache.GetStringAsync($"papers-by-author-{authorId}");
                if (!string.IsNullOrEmpty(cachedPapers))
                {
                    // Return cached data
                    Console.WriteLine("From cache");
                    return JsonSerializer.Deserialize<IEnumerable<Paper>>(cachedPapers);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting data from cache: {ex.Message}");
            }

            // Get data from the database
            var papers = await _papersCollection.Find(x => x.AuthorId == authorId).ToListAsync();
            Console.WriteLine("From DB");

            try
            {
                // Cache the data
                await _cache.SetStringAsync($"papers-by-author-{authorId}", JsonSerializer.Serialize(papers), _cacheOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error caching data: {ex.Message}");
            }

            return papers;
        }

        public async Task<Paper> GetPaperAsync(string id)
        {
            try
            {
                var cachedPaper = await _cache.GetStringAsync($"paper-{id}");
                if (!string.IsNullOrEmpty(cachedPaper))
                {
                    // Return cached data
                    Console.WriteLine("From cache");
                    return JsonSerializer.Deserialize<Paper>(cachedPaper);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting data from cache: {ex.Message}");
            }

            // Get data from the database
            var paper = await _papersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            Console.WriteLine("From DB");

            if (paper != null)
            {
                try
                {
                    // Cache the data
                    await _cache.SetStringAsync($"paper-{id}", JsonSerializer.Serialize(paper), _cacheOptions);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error caching data: {ex.Message}");
                }
            }

            return paper;
        }


        public async void RemovePaper(string id)
        {
            try
            {
                await _papersCollection.DeleteOneAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding paper to MongoDB: {ex.Message}");
            }

            try
            {
                await _cache.RemoveAsync("all-papers");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing all-papers from cache: {ex.Message}");
            }
        }

        public async void RemovePaperByAuthorId(int id)
        {
            try
            {
                await _papersCollection.DeleteManyAsync(x => x.AuthorId == id);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding paper to MongoDB: {ex.Message}");
            }

            try
            {
                await _cache.RemoveAsync("all-papers");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing all-papers from cache: {ex.Message}");
            }
        }

        public async void UpdatePaper(Paper paper)
        {
            try
            {
                await _papersCollection.ReplaceOneAsync(x => x.Id == paper.Id, paper);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding paper to MongoDB: {ex.Message}");
            }

            try
            {
                await _cache.RemoveAsync("all-papers");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing all-papers from cache: {ex.Message}");
            }
        }
    }
}
