using UserService.Dto;
using UserService.Model;

namespace UserService.Service
{
    public interface IUserService
    {
        public IEnumerable<User> getAllUser();
        public User getUserById(int id);

        public UserLoginInfoDto getUserByEmail(string email);

        public void addUser(User user);

        public Boolean removeUser(int id);

    }
}
