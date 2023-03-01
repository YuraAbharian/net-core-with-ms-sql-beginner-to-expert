using DotnetApi.Data;
using DotnetApi.Dtos;
using DotnetApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    DataContextDapper _dapper;
    public UserController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }

    [HttpGet("GetUsers")]
    public IEnumerable<User> GetUsers()
    {
        return _dapper.LoadData<User>("SELECT * FROM TutorialAppSchema.Users");
    }

    [HttpGet("GetSingleUser/{userId}")]
    public User GetSingleUser(int userId)
    {
        return _dapper.LoadSingleData<User>($"SELECT * FROM TutorialAppSchema.Users AS U WHERE U.UserId={userId}");
    }

    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        string query = $@"
            UPDATE TutorialAppSchema.Users
            SET [FirstName] = '{user.FirstName}',
                [LastName] = '{user.LastName}',
                [Email] = '{user.Email}',
                [Gender] = '{user.Gender}',
                [Active] = '{user.Active}'
            WHERE UserId = {user.UserId}
        ";

        bool result = _dapper.ExecuteSql(query);

        if (result)
        {
            return Ok();
        }
        throw new Exception("Failed to Update User");
    }

    [HttpPost("AddUser")]
    public IActionResult AddUser(UserToAddDto user)
    {
                string query = $@"
            INSERT INTO TutorialAppSchema.Users (
                [FirstName],
                [LastName],
                [Email],
                [Gender],
                [Active]
           ) VALUES ('{user.FirstName}', '{user.LastName}', '{user.Email}', '{user.Gender}', '{user.Active}')
        ";

        bool result = _dapper.ExecuteSql(query);

        if (result)
        {
            return Ok();
        }
        throw new Exception("Failed to Add User");
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        string query = $"DELETE FROM TutorialAppSchema.Users WHERE UserId = '{userId}'";

        bool result = _dapper.ExecuteSql(query);

        if (result)
        {
            return Ok();
        }
        throw new Exception("Failed to Delete User");
    }
}
