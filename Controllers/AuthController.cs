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

                string passwordSaltPlusString = _config.GetSection("AppSettings:PasswordKey").Value + Convert.ToBase64String(passwordSalt);

                byte[] passwordHash = KeyDerivation.Pbkdf2(
                password: userForRegistration.Password,
                salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 1000000,
                numBytesRequested: 256 / 8
            );

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
                        return Ok();
                    }

                throw new Exception($"Cannot register a user");
            }
            throw new Exception($"User with email = '{userForRegistration.Email}' already exists");
        }

        throw new Exception("Password are not the same");
    }
    [HttpPost("Login")]
    public IActionResult Login(UserForLoginDto uerForLoginDto)
    {
        return Ok();
    }
}
