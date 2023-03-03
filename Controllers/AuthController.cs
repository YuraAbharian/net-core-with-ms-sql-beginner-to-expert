using System.Data;
using System.Security.Cryptography;
using DotnetApi.Data;
using DotnetApi.Dtos;
using DotnetApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using DotnetApi.Helpers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly DataContextDapper _dapper;
    private readonly AuthHelper _authHelper;
    public AuthController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
        _authHelper = new AuthHelper(config);
    }

    [HttpPost("Register")]
    public IActionResult Register(UserForRegistrationDto userForRegistration)
    {
        if (userForRegistration.Password == userForRegistration.PasswordConfirm)
        {
            string getUserByEmaiSql = $"SELECT Email FROM TutorialAppSchema.Auth WHERE Email = '{userForRegistration.Email}'";
            IEnumerable<string> existingUsers = _dapper.LoadData<string>(getUserByEmaiSql);

            if (existingUsers.Count() == 0)
            {
                byte[] passwordSalt = new byte[128 / 8];
                using (RandomNumberGenerator randomNumber = RandomNumberGenerator.Create())
                {
                    randomNumber.GetNonZeroBytes(passwordSalt);
                }

                byte[] passwordHash = _authHelper.HashPassword(userForRegistration.Password, passwordSalt);

                string InsertAccountSql = @"
                        INSERT INTO TutorialAppSchema.Auth  ([Email],
                        [PasswordHash],
                        [PasswordSalt]) VALUES ('" + userForRegistration.Email +
                        "', @PasswordHash, @PasswordSalt)";

                List<SqlParameter> sqlParameters = new List<SqlParameter>();

                SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSalt", SqlDbType.VarBinary);
                passwordSaltParameter.Value = passwordSalt;

                SqlParameter passwordHashParameter = new SqlParameter("@PasswordHash", SqlDbType.VarBinary);
                passwordHashParameter.Value = passwordHash;

                sqlParameters.Add(passwordSaltParameter);
                sqlParameters.Add(passwordHashParameter);

                if (_dapper.ExecuteSqlWithParameters(InsertAccountSql, sqlParameters))
                {
                    string query = $@"
            INSERT INTO TutorialAppSchema.Users (
                [FirstName],
                [LastName],
                [Email],
                [Gender],
                [Active]
           ) VALUES ('{userForRegistration.FirstName}', '{userForRegistration.LastName}', '{userForRegistration.Email}', '{userForRegistration.Gender}', 1)
        ";

                    bool result = _dapper.ExecuteSql(query);

                    if (result)
                    {
                        return Ok();
                    }
                    throw new Exception("Failed to Register User");
                }

                throw new Exception($"Cannot register a user");
            }
            throw new Exception($"User with email = '{userForRegistration.Email}' already exists");
        }

        throw new Exception("Password are not the same");
    }
    [HttpPost("Login")]
    public IActionResult Login(UserForLoginDto userForLoginDto)
    {
        string getUserByEmailFromAuthSql = $"SELECT * FROM TutorialAppSchema.Auth WHERE Email = '{userForLoginDto.Email}'";
        UserForLoginConfirmationDto userAuthData = _dapper.LoadSingleData<UserForLoginConfirmationDto>(getUserByEmailFromAuthSql);
        byte[] passwordHash = _authHelper.HashPassword(userForLoginDto.Password, userAuthData.PasswordSalt);

        for (int i = 0; i < passwordHash.Length; i++)
        {
            if (passwordHash[i] != userAuthData.PasswordHash[i])
            {
                return StatusCode(401, "Incorrect password!");
            }
        }
        string getUserByEmailFromUsersSql = $"SELECT * FROM TutorialAppSchema.Users WHERE Email = '{userForLoginDto.Email}'";
        User user = _dapper.LoadSingleData<User>(getUserByEmailFromUsersSql);

        return Ok(new Dictionary<string, string> {
            {"token", _authHelper.CreateToken(user.UserId)}
        });
    }

    [HttpGet("RefreshToken")]
    public string RefreshToken()
    {
        string userSql = $"SELECT UserId FROM TutorialAppSchema.Users WHERE UserId = '{User.FindFirst("UserId")?.Value}'";

        int userId = _dapper.LoadSingleData<int>(userSql);

        return _authHelper.CreateToken(userId);
    }

}
