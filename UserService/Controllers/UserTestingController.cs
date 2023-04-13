using Microsoft.AspNetCore.Mvc;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/user/testing")]
    public class UserTestingController : ControllerBase
    {
        [HttpGet]
        [Route("public")]
        public ActionResult<String> TestReader()
        {
            return Ok("Hello World!");
        }
    }
}
