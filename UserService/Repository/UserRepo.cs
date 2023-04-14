using UserService.Data;
using UserService.Model;

namespace UserService.Repository
{
    public class UserRepo : IUserRepo
    {
        private readonly AppDbContext _context;

        public UserRepo(AppDbContext context)
        {
            _context = context;
        }

        public void AddUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            _context.Users.Add(user);
        }

        public void DeleteUserById(int id)
        {
            User del = _context.Users.FirstOrDefault(x => x.Id == id);
            if (del != null)
            {
                _context.Users.Remove(del);
            }
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        public User GetById(int id)
        {
            return _context.Users.FirstOrDefault(p => p.Id == id);
        }

        public User GetByEmail(string email)
        {
            return _context.Users.FirstOrDefault(p => p.Email == email);
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}
