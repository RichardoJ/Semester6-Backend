using CatalogService.Model;

namespace CatalogService.Repository
{
    public interface ICategoryRepo
    {
        void AddCategory(Category category);

        void RemoveCategory(int id);

        Category GetCategory(int id);

        IEnumerable<Category> GetAllCategories();

        bool SaveChanges();
    }
}
