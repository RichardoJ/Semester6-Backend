using Microsoft.EntityFrameworkCore;
using PublishService.Data;
using PublishService.Model;

namespace PublishService.Repository
{
    public class PaperRepository : IPaperRepository
    {
        private readonly AppDbContext _context;

        public PaperRepository(AppDbContext context)
        {
            _context = context;
        }

        public void AddPaper(Paper book)
        {
            if (book == null) throw new ArgumentNullException(nameof(book));
            _context.papers.Add(book);
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
        }

        public Paper GetPaper(int id)
        {
            var content = _context.papers.FirstOrDefault(x => x.Id == id);
            if (content != null)
            {
                _context.Entry(content).State = EntityState.Detached;
            }
            return content;
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }

        public IEnumerable<Paper> GetAllPapers()
        {
            return _context.papers.ToList();
        }

        public void UpdatePaper(Paper paper)
        {
            if (paper == null) throw new ArgumentNullException(nameof(paper));
            _context.papers.Update(paper);
        }

        public Boolean GetPaperByTitle(string title)
        {
            return _context.papers.Any(p => p.Title == title);
        }
    }
}
