using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PublishNoSQL.Model;
using System.Security.Authentication;

namespace PublishNoSQL.Repository
{
    public class PaperRepository : IPaperRepository
    {
        private IMongoCollection<Paper> _papersCollection;

        public PaperRepository(IOptions<PaperStoreDatabaseSettings> settings)
        {
            MongoClientSettings mongoSettings = MongoClientSettings.FromUrl(new MongoUrl(settings.Value.ConnectionString));
            mongoSettings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
            var mongoClient = new MongoClient(mongoSettings);
            //var mongoClient = new MongoClient(settings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);

            _papersCollection = mongoDatabase.GetCollection<Paper>(settings.Value.PapersCollectionName);
            Console.WriteLine(settings.Value.ConnectionString);
            Console.WriteLine(settings.Value.DatabaseName);
            Console.WriteLine(settings.Value.PapersCollectionName);
        }

        public async void AddPaper(Paper paper)
        {
            await _papersCollection.InsertOneAsync(paper);
        }

        public async Task<Paper> GetPaperAsync(string id)
        {
            var paper = await _papersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            return paper;
        }

        public async void RemovePaper(string id)
        {
            await _papersCollection.DeleteOneAsync(x => x.Id == id);
        }

        public async void RemovePaperByAuthorId(int id)
        {
            await _papersCollection.DeleteManyAsync(x => x.AuthorId == id);
        }

        public async void UpdatePaper(Paper paper)
        {
            await _papersCollection.ReplaceOneAsync(x => x.Id == paper.Id, paper);
        }
    }
}
