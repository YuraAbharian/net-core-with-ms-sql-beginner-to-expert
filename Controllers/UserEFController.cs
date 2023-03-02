using AutoMapper;
using DotnetApi.Data;
using DotnetApi.Dtos;
using DotnetApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UserEFController : ControllerBase
{
    private readonly DataContextEF _entityFramework;
    private readonly Mapper _mapper;
    public UserEFController(IConfiguration config)
    {
        _entityFramework = new DataContextEF(config);
        _mapper = new Mapper(new MapperConfiguration((cfg) => {
            cfg.CreateMap<UserToAddDto, User>();
        }));
    }

    [HttpGet("GetUsers")]
    public IEnumerable<User> GetUsers()
    {
        return _entityFramework.Users.ToList<User>();
    }

    [HttpGet("GetSingleUser/{userId}")]
    public User GetSingleUser(int userId)
    {
        User? user = _entityFramework
        .Users
        .Where(u => u.UserId == userId)
        .FirstOrDefault<User>();

        if (user != null)
        {
            return user;
        }

        throw new Exception($"Failed get user by id'${userId}'");
    }

    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        User? DbUser = _entityFramework
           .Users
           .Where(u => u.UserId == user.UserId)
           .FirstOrDefault<User>();

        if (DbUser != null)
        {
            DbUser.FirstName = user.FirstName;
            DbUser.LastName = user.LastName;
            DbUser.Gender = user.Gender;
            DbUser.Active = user.Active;
            DbUser.Email = user.Email;

            int result = _entityFramework.SaveChanges();

            if (result > 0)
            {
                return Ok();
            }
            throw new Exception("Failed to Update User");
        }

        throw new Exception($"Failed get user by id'${user.UserId}'");
    }

    [HttpPost("AddUser")]
    public IActionResult AddUser(UserToAddDto user)
    {
        User? DbUser = _mapper.Map<User>(user);

        _entityFramework.Add<User>(DbUser);

        int result = _entityFramework.SaveChanges();

        if (result > 0)
        {
            return Ok();
        }
        throw new Exception("Failed to Add User");
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
       User? DbUser = _entityFramework
           .Users
           .Where(u => u.UserId == userId)
           .FirstOrDefault<User>();

        if (DbUser != null)
        {
            _entityFramework.Users.Remove(DbUser);

            int result = _entityFramework.SaveChanges();

            if (result > 0)
            {
                return Ok();
            }
            throw new Exception("Failed to Delete User");
        }

        throw new Exception($"Failed get user by id'${userId}'");
    }
}
