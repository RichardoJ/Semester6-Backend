using PublishService.Model;

namespace PublishService.Repository
{
    public interface ICategoryRepository
    {
        void AddCategory(Category category);

        void RemoveCategory(int id);

        Category GetCategory(int id);

        IEnumerable<Category> GetAllCategories();

        bool SaveChanges();
    }
}
