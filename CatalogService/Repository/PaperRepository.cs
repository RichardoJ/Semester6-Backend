using CatalogService.Data;
using CatalogService.Model;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Repository
{
    public class PaperRepository : IPaperRepository
    {
        private readonly AppDbContext _context;

        public PaperRepository(AppDbContext context)
        {
            _context = context;
        }

        public void AddPaper(Paper paper)
        {
            if (paper == null) throw new ArgumentNullException(nameof(paper));
            _context.papers.Add(paper);
        }

        public void RemovePaper(int id)
        {
            Paper del = _context.papers.FirstOrDefault(x => x.Id == id);
            if (del != null)
            {
                _context.papers.Remove(del);
            }
        }

        public void RemovePaperByAuthorId(int id)
        {
            var userID = id;
            var deletedPaper = _context.papers.Where(t => t.AuthorId == userID).ToList();
            _context.papers.RemoveRange(deletedPaper);
            //_context.papers.FromSql($"DELETE FROM paper WHERE AuthorId = {userID}");
            //return _context.papers.Where(t => t.AuthorId == userID).ExecuteDeleteAsync;
        }

        public Paper GetPaper(int id)
        {
            return _context.papers.Include(i => i.Category).FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<Paper> GetAllPapers()
        {
            return _context.papers.Include(i => i.Category).ToList();
        }

        public IEnumerable<Paper> GetAllPapersPagination(Parameters parameters)
        {
            return _context.papers.Include(i => i.Category).Skip((parameters.PageNumber - 1) * parameters.PageSize).Take(parameters.PageSize).ToList();
        }


        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }

        public bool PaperExist(string name)
        {
            return _context.papers.Any(p => p.Title == name);
        }

        public void UpdatePaper(Paper paper)
        {
            _context.papers.Update(paper);
        }

        public IEnumerable<Paper> GetAllPapersByAuthor(int authorId)
        {
            return _context.papers.Include(i => i.Category).Where(x => x.AuthorId == authorId).ToList();
        }
    }
}
