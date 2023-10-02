using Microsoft.AspNetCore.Mvc;
using WareWiz.Models;

namespace WareWiz.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        [HttpGet(Name = "GetUserById")]
        public User Get()
        {
            return new User();
        }
    }
}