using System.Data;
using System.Security.Cryptography;
using System.Text;
using DotnetApi.Data;
using DotnetApi.Dtos;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    DataContextDapper _dapper;
    IConfiguration _config;
    public AuthController(IConfiguration config)
    {
        _config = config;
        _dapper = new DataContextDapper(config);
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

                byte[] passwordHash = HashPassword(userForRegistration.Password, passwordSalt);

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
        string getUserByEmaiSql = $"SELECT * FROM TutorialAppSchema.Auth WHERE Email = '{userForLoginDto.Email}'";
        UserForLoginConfirmationDto userAuthData = _dapper.LoadSingleData<UserForLoginConfirmationDto>(getUserByEmaiSql);
        byte[] passwordHash = HashPassword(userForLoginDto.Password, userAuthData.PasswordSalt);

        for (int i = 0; i < passwordHash.Length; i++)
        {
            if (passwordHash[i] != userAuthData.PasswordHash[i])
            {
                return StatusCode(401, "Incorrect password!");
            }
        }

        return Ok();
    }

    private byte[] HashPassword(string password, byte[] passwordSalt)
    {
        string passwordSaltPlusString = _config.GetSection("AppSettings:PasswordKey")
        .Value + Convert.ToBase64String(passwordSalt);

        return KeyDerivation.Pbkdf2(
            password: password,
            salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 1000000,
            numBytesRequested: 256 / 8
        );
    }
}
