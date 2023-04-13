namespace CatalogNoSQL.Model
{
    public class PaperStoreDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string PapersCollectionName { get; set; } = null!;
    }
}
