using AutoMapper;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Dto;
using UserService.RabbitMQ;
using UserService.Service;

namespace UserService.Controllers
{

    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {


        private readonly IUserService _service;
        private readonly IMapper _mapper;
        private readonly IMessageBusClient _messageBusClient;

        public UserController(IUserService userService, IMapper mapper, IMessageBusClient messageBusClient)
        {
            _service = userService;
            _mapper = mapper;
            _messageBusClient = messageBusClient;
            //firebaseClient= new FirebaseAuthClient(config);
        }

        [HttpGet]
        [Route("all")]
        public ActionResult<IEnumerable<UserDto>> GetUsers()
        {
            Console.WriteLine("Getting Users....");

            var users = _service.getAllUser();
            return Ok(_mapper.Map<IEnumerable<UserDto>>(users));
        }

        [HttpGet("{id}")]
        public ActionResult<UserDto> GetUser(int id)
        {
            Console.WriteLine("Getting user by id ...");

            var oneUser = _service.getUserById(id);
            if (oneUser == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(_mapper.Map<UserDto>(oneUser));
            }
        }

        [HttpPost]
        public ActionResult createUser(UserDto user)
        {
            Console.WriteLine("Creating User....");
            var saveUser = _mapper.Map<Model.User>(user);
            _service.addUser(saveUser);
            return Ok(user);
        }

        //[HttpPost]
        //[Route("login")]
        //public async Task<ActionResult> LoginUser(UserDto user)
        //{
        //    Console.WriteLine("Login User....");
        //    var userCredentials = await firebaseClient.SignInWithEmailAndPasswordAsync(user.Email, user.Password);
        //    return Ok(user);
        //}

        [HttpPost]
        [Route("signup")]
        public async Task<ActionResult> signUpUserAsync(UserDto user)
        {
            Console.WriteLine("Sign Up User....");
            var saveUser = _mapper.Map<Model.User>(user);
            _service.addUser(saveUser);
            UserRecordArgs args = new UserRecordArgs()
            {
                Email = user.Email,
                Password = user.Password,
                DisplayName = user.Name,
            };
            UserRecord userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(args);
            var claims = new Dictionary<string, object>()
            {
                { "role", user.Role },
            };
            FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(userRecord.Uid, claims);
            return Ok(userRecord);
        }

        [HttpDelete("{id}")]
        public ActionResult deleteUser(int id)
        {
            Console.WriteLine($"deleted {id}");
            //_service.removeUser(id);
            var deletedUser = new UserDeleteDto();
            deletedUser.Id = id;
            deletedUser.Event = "Delete_user";
            //Send Async Message
            try
            {
                _messageBusClient.deleteAuthor(deletedUser);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not send message async: {ex.Message}");
            }
            return Ok("Success");
            //if (_service.getUserById(id) == null)
            //{
            //    //Send Async Message
            //    try
            //    {
            //        _messageBusClient.deleteAuthor(deletedUser);
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine($"Could not send message async: {ex.Message}");
            //    }
            //    return Ok("Success");
            //}
            //else
            //{
            //    return Ok(NotFound());
            //}
        }

        [HttpGet]
        [Route("reader")]
        [Authorize(Policy = "Public")]
        public ActionResult<String> TestReader()
        {
            Console.WriteLine("Testing Authentication and Authorization ...");
            return Ok("Hello Reader");
        }

        [HttpGet]
        [Route("admin")]
        [Authorize(Policy = "EmployeeOnly")]
        public ActionResult<String> TestAdmin()
        {
            Console.WriteLine("Testing Authentication and Authorization ...");
            return Ok("Hello Admin");
        }

    }
}
