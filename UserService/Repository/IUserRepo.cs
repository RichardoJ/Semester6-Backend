using UserService.Model;

namespace UserService.Repository
{
    public interface IUserRepo
    {
        bool SaveChanges();

        void AddUser(User user);

        IEnumerable<User> GetAllUsers();

        User GetById(int id);

        User GetByEmail(string email);

        void DeleteUserById(int id);
    }
}
