using FirebaseAdmin.Auth;

namespace UserService.Dto
{
    public class UserSignInfoDto
    {
        public int Id { get; set; }

        public UserRecord User { get; set; }
    }
}
