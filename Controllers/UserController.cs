using Microsoft.AspNetCore.Mvc;

namespace DotnetApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    [HttpGet("GetUsers/{testValue}")]
    public IEnumerable<string> GetUsers(string testValue)
    {
        return new string[]
        {
            "User1", "User2", testValue, "New User"
        };
    }
}
