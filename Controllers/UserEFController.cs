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
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    public UserEFController(IConfiguration config, IUserRepository userRepository)
    {
        _userRepository = userRepository;
        _entityFramework = new DataContextEF(config);
        _mapper = new Mapper(new MapperConfiguration((cfg) =>
        {
            cfg.CreateMap<UserToAddDto, User>();
        }));
    }

    [HttpGet("GetUsers")]
    public IEnumerable<User> GetUsers()
    {
        return _userRepository.GetUsers();
    }

    [HttpGet("GetSingleUser/{userId}")]
    public User GetSingleUser(int userId)
    {
        User? user = _userRepository.GetSingleUser(userId);

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

            if (_userRepository.SaveChanges())
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

        _userRepository.AddEntity<User>(DbUser);

        if (_userRepository.SaveChanges())
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
            _userRepository.RemoveEntity<User>(DbUser);

            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            throw new Exception("Failed to Delete User");
        }

        throw new Exception($"Failed get user by id'${userId}'");
    }

    // UserJobInfo
    [HttpGet("GetSingleUserJobInfo/{userId}")]
    public UserJobInfo GetSingleUserJobInfo(int userId)
    {
        return _userRepository.GetSingleUserJobInfo(userId);
    }

    [HttpPut("EditUserJobInfo")]
    public IActionResult EditUserJobInfo(UserJobInfoToAddDto user)
    {
        UserJobInfo? DbUserJobInfo = _userRepository.GetSingleUserJobInfo(user.UserId);

        if (DbUserJobInfo != null)
        {
            DbUserJobInfo.Department = user.Department;
            DbUserJobInfo.JobTitle = user.JobTitle;

            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            throw new Exception("Failed to Update UserJobInfo");
        }

        throw new Exception($"Failed get user job info by id'${user.UserId}'");
    }

    [HttpPost("AddUserJobInfo")]
    public IActionResult AddUserJobInfo(UserJobInfoToAddDto user)
    {
        User? DbUser = _mapper.Map<User>(user);

        _userRepository.AddEntity<User>(DbUser);

        if (_userRepository.SaveChanges())
        {
            return Ok();
        }
        throw new Exception("Failed to Add UserJobInfo");
    }

    [HttpDelete("DeleteUserJobInfo/{userId}")]
    public IActionResult DeleteUserJobInfo(int userId)
    {
        UserJobInfo? DbUserJobInfo = _userRepository.GetSingleUserJobInfo(userId);

        if (DbUserJobInfo != null)
        {
            _userRepository.RemoveEntity<UserJobInfo>(DbUserJobInfo);

            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            throw new Exception("Failed to Delete UserJobInfo");
        }

        throw new Exception($"Failed get user job info by id'${userId}'");
    }

    // UserSalary
    [HttpGet("GetSingleUserSalary/{userId}")]
    public UserSalary GetSingleUserSalary(int userId)
    {
        return _userRepository.GetSingleUserSalary(userId);
    }

    [HttpPut("EditUserSalary")]
    public IActionResult EditUserSalary(UserSalary user)
    {
        UserSalary? DbUserSalary = _userRepository.GetSingleUserSalary(user.UserId);

        if (DbUserSalary != null)
        {
            DbUserSalary.Salary = user.Salary;

            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            throw new Exception("Failed to Update UserSalary");
        }

        throw new Exception($"Failed get user salary by id'${user.UserId}'");
    }

    [HttpPost("AddUserSalary")]
    public IActionResult AddUserSalary(UserSalaryToAddDto user)
    {
        UserSalary? DbUser = _mapper.Map<UserSalary>(user);

        _userRepository.AddEntity<UserSalary>(DbUser);

        if (_userRepository.SaveChanges())
        {
            return Ok();
        }
        throw new Exception("Failed to Add UserSalary");
    }

    [HttpDelete("DeleteUserSalary/{userId}")]
    public IActionResult DeleteUserSalary(int userId)
    {
        UserSalary? DbUserSalary = _userRepository.GetSingleUserSalary(userId);

        if (DbUserSalary != null)
        {
            _userRepository.RemoveEntity<UserSalary>(DbUserSalary);

            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            throw new Exception("Failed to Delete UserSalary");
        }

        throw new Exception($"Failed get user salary by id'${userId}'");
    }
}
