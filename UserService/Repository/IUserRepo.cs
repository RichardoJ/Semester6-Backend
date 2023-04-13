using UserService.Model;

namespace UserService.Repository
{
    public interface IUserRepo
    {
        bool SaveChanges();

        void AddUser(User user);

        IEnumerable<User> GetAllUsers();

        User GetById(int id);

        void DeleteUserById(int id);
    }
}
