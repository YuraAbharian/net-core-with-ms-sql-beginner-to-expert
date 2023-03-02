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

    // UserSalary
    [HttpGet("GetUsersSalary")]
    public IEnumerable<UserSalary> GetUsersSalary()
    {
        return _dapper.LoadData<UserSalary>("SELECT * FROM TutorialAppSchema.UserSalary");
    }

    [HttpGet("GetSingleUserSalary/{userId}")]
    public UserSalary GetSingleUserSalary(int userId)
    {
        return _dapper.LoadSingleData<UserSalary>($"SELECT * FROM TutorialAppSchema.UserSalary AS U WHERE U.UserId={userId}");
    }

    [HttpPut("EditUserSalary")]
    public IActionResult EditUserSalary(UserSalaryToAddDto user)
    {
        string query = $@"
            UPDATE TutorialAppSchema.UserSalary
            SET [Salary] = '{user.Salary}'
            WHERE UserId = {user.UserId}
        ";

        bool result = _dapper.ExecuteSql(query);

        if (result)
        {
            return Ok();
        }
        throw new Exception("Failed to Update UserSalary");
    }

    [HttpPost("AddUserSalary")]
    public IActionResult AddUserSalary(UserSalaryToAddDto user)
    {
        string query = $@"
            INSERT INTO TutorialAppSchema.UserSalary (
                [Salary],
                [UserId]
           ) VALUES ('{user.Salary}', '{user.UserId}')
        ";

        bool result = _dapper.ExecuteSql(query);

        if (result)
        {
            return Ok();
        }
        throw new Exception("Failed to Add UserSalary");
    }

    [HttpDelete("DeleteUserSalary/{userId}")]
    public IActionResult DeleteUserSalary(int userId)
    {
        string query = $"DELETE FROM TutorialAppSchema.UserSalary WHERE UserId = '{userId}'";

        bool result = _dapper.ExecuteSql(query);

        if (result)
        {
            return Ok();
        }
        throw new Exception("Failed to Delete UserSalary");
    }

    // UserJobInfo
    [HttpGet("GetUserJobInfo")]
    public IEnumerable<UserJobInfo> GetUserJobInfo()
    {
        return _dapper.LoadData<UserJobInfo>("SELECT * FROM TutorialAppSchema.UserJobInfo");
    }

    [HttpGet("GetSingleUserJobInfo/{userId}")]
    public UserJobInfo GetSingleUserJobInfo(int userId)
    {
        return _dapper.LoadSingleData<UserJobInfo>($"SELECT * FROM TutorialAppSchema.UserJobInfo AS U WHERE U.UserId={userId}");
    }

    [HttpPut("EditUserJobInfo")]
    public IActionResult EditUserJobInfo(UserJobInfo user)
    {
        string query = $@"
            UPDATE TutorialAppSchema.UserJobInfo
            SET [Department] = '{user.Department}',
                [JobTitle] = '{user.JobTitle}'
            WHERE UserId = {user.UserId}
        ";

        bool result = _dapper.ExecuteSql(query);

        if (result)
        {
            return Ok();
        }
        throw new Exception("Failed to Update UserJobInfo");
    }

    [HttpPost("AddUserJobInfo")]
    public IActionResult AddUserJobInfo(UserJobInfo user)
    {
        string query = $@"
            INSERT INTO TutorialAppSchema.UserJobInfo (
                [UserId],
                [Department],
                [JobTitle]
           ) VALUES ('{user.UserId}','{user.Department}', '{user.JobTitle}')
        ";

        bool result = _dapper.ExecuteSql(query);

        if (result)
        {
            return Ok();
        }
        throw new Exception("Failed to Add UserJobInfo");
    }

    [HttpDelete("DeleteUserJobInfo/{userId}")]
    public IActionResult DeleteUserJobInfo(int userId)
    {
        string query = $"DELETE FROM TutorialAppSchema.UserJobInfo WHERE UserId = '{userId}'";

        bool result = _dapper.ExecuteSql(query);

        if (result)
        {
            return Ok();
        }
        throw new Exception("Failed to Delete UserJobInfo");
    }


}
