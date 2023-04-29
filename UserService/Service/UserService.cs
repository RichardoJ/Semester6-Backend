using UserService.Dto;
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

        public UserLoginInfoDto getUserByEmail(string email)
        {
            var userInfo = _userRepo.GetByEmail(email);
            if (userInfo != null)
            {
                return new UserLoginInfoDto
                {
                    Id = userInfo.Id,
                    Email = userInfo.Email,
                };
            }
            else
            {
                return null;
        }
            }

        public void addUser(User user)
        {
            _userRepo.AddUser(user);
            _userRepo.SaveChanges();
        }

        public Boolean removeUser(int id)
        {
            var user = _userRepo.GetById(id);
            if (user != null)
            {
                _userRepo.DeleteUserById(id);
                _userRepo.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
            
        }
    }
}
