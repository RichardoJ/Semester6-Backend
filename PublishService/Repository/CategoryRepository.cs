using PublishService.Data;
using PublishService.Model;

namespace PublishService.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public void AddCategory(Category category)
        {
            if (category == null) throw new ArgumentNullException();
            _context.categories.Add(category);
        }

        public IEnumerable<Category> GetAllCategories()
        {
            return _context.categories.ToList();
        }

        public Category GetCategory(int id)
        {
            return _context.categories.FirstOrDefault(c => c.Id == id);
        }

        public void RemoveCategory(int id)
        {
            Category del = _context.categories.FirstOrDefault(x => x.Id == id);
            if (del != null)
            {
                _context.categories.Remove(del);
            }
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}
