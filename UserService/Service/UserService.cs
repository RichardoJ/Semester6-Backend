using UserService.Model;
using UserService.Repository;

namespace UserService.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepo _userRepo;

        public UserService(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        public IEnumerable<User> getAllUser()
        {
            return _userRepo.GetAllUsers();
        }

        public User getUserById(int id)
        {
            return _userRepo.GetById(id);
        }

        public void addUser(User user)
        {
            _userRepo.AddUser(user);
            _userRepo.SaveChanges();
        }

        public void removeUser(int id)
        {
            _userRepo.DeleteUserById(id);
            _userRepo.SaveChanges();
        }
    }
}
