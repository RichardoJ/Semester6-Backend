using CatalogService.Data;
using CatalogService.Model;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Repository
{
    public class CategoryRepo : ICategoryRepo
    {
        private readonly AppDbContext _context;

        public CategoryRepo(AppDbContext context)
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
