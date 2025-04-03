using System.Data;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ProjectManagerAPI.Data;
using ProjectManagerAPI.Dtos;
using ProjectManagerAPI.Helpers;

namespace ProjectManagerAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    // Private attributes 
    private readonly DataContextDapper _dapper;
    private readonly AuthHelper _authHelper;

    // Constructor 
    public AuthController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
        _authHelper = new AuthHelper(config);
    }

    // User Registration endpoint 
    [AllowAnonymous]
    [HttpPost("Register")]
    public IActionResult Register(UserForRegistrationDto userForRegistration)
    {
        if (userForRegistration.Password == userForRegistration.PasswordConfirmation)
        {
            string sqlCheckUserExists = "SELECT Email FROM ProjectManagerSchema.Auth WHERE Email = '"
                + userForRegistration.Email + "'";
            IEnumerable<string> existingUser = _dapper.LoadData<string>(sqlCheckUserExists);
            Console.WriteLine(sqlCheckUserExists);

            if (existingUser.Count() == 0)
            {
                //Creation of the Password Salt(Random value added to the password before hashing) 
                //and Password Hash (The result of running a password through a cryptographic hashing algorithm)
                byte[] passwordSalt = new byte[128 / 8];
                using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                {
                    rng.GetNonZeroBytes(passwordSalt);
                }

                byte[] passwordHash = _authHelper.GetPasswordHash(userForRegistration.Password, passwordSalt);

                // SQL Query used to add the registered user in the Auth Table that contains the email and the password
                string sqlAddAuth = @"
                    INSERT INTO ProjectManagerSchema.Auth (
                        [Email],
                        [PasswordHash],
                        [PasswordSalt]
                    ) VALUES ('" + userForRegistration.Email +
                "', @PasswordHash, @PasswordSalt)";

                // Parameters used in the parameterized SQL Query used to add the registered user in the Auth Table
                List<SqlParameter> sqlParameters = new List<SqlParameter>();

                SqlParameter passwordSaltParameter = new("@PasswordSalt", SqlDbType.VarBinary);
                passwordSaltParameter.Value = passwordSalt;

                SqlParameter passwordHashParameter = new("@PasswordHash", SqlDbType.VarBinary);
                passwordHashParameter.Value = passwordHash;

                sqlParameters.Add(passwordSaltParameter);
                sqlParameters.Add(passwordHashParameter);

                //Execution of the SQL Query that adds the registered used into the Auth Table
                if (_dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParameters))
                {
                    // SQL Query used to Add the user into the Users Table after the registration is complete
                    string sqlAddUser = @"
                        INSERT INTO ProjectManagerSchema.Users(
                            [FirstName],
                            [LastName],
                            [Email]
                        ) VALUES (" +
                        "'" + userForRegistration.FirstName +
                        "', '" + userForRegistration.LastName +
                        "', '" + userForRegistration.Email + "')";

                    //Execution of the SQL Query that adds the registered used into the Users Table
                    if (_dapper.ExecuteSql(sqlAddUser))
                    {
                        return Ok();
                    }
                    throw new Exception("Failed to add user!");
                }
                throw new Exception("Failed to register user!");
            }
            return StatusCode(409, "Email Already Exists!");
        }
        return StatusCode(409, "Passwords do not match!");
    }

    // User Login endpoint
    [AllowAnonymous]
    [HttpPost("Login")]
    public IActionResult Login(UserForLoginDto userForLogin)
    {
        string sqlForHashAndSalt = @"SELECT [PasswordHash], [PasswordSalt] FROM ProjectManagerSchema.Auth
            WHERE Email = '" + userForLogin.Email + "'";

        UserForLoginConfirmationDto userForLoginConfirmation;

        // Try to find if the entered email exists in the database and return an error in case it's invalid
        try
        {
            userForLoginConfirmation = _dapper.LoadSingleData<UserForLoginConfirmationDto>(sqlForHashAndSalt);
        }
        catch (InvalidOperationException)
        {
            return StatusCode(404, "Incorrect Email!");
        }

        byte[] passwordHash = _authHelper.GetPasswordHash(userForLogin.Password, userForLoginConfirmation.PasswordSalt);

        // Compare the encrypted password from the database with the password that was introduced to see if they are the same
        for (int index = 0; index < passwordHash.Length; index++)
        {
            if (passwordHash[index] != userForLoginConfirmation.PasswordHash[index])
            {
                return StatusCode(401, "Incorrect Password!");
            }
        }

        // Return the userId of the logged user in order to create a JWT
        string userIdSql = "SELECT UserId FROM ProjectManagerSchema.Users WHERE Email = '" + userForLogin.Email + "'";
        string firstNameSql = "SELECT firstName FROM ProjectManagerSchema.Users WHERE Email = '" + userForLogin.Email + "'";
        string lastNameSql = "SELECT lastName FROM ProjectManagerSchema.Users WHERE Email = '" + userForLogin.Email + "'";

        int userId = _dapper.LoadSingleData<int>(userIdSql);
        string firstName = _dapper.LoadSingleData<string>(firstNameSql);
        string lastName = _dapper.LoadSingleData<string>(lastNameSql);

        // Return the Ok response and the JWT
        return Ok(new Dictionary<string, string>() {
            {"token", _authHelper.CreateToken(userId, firstName, lastName)}
        });
    }

    // Endpoint used to refresh the JWT
    [HttpGet("RefreshToken")]
    public IActionResult RefreshToken()
    {
        string userId = User.FindFirst("userId")?.Value + "";

        string userIdSql = "SELECT userId FROM ProjectManagerSchema.Users WHERE UserId = " + userId;
        string firstNameSql = "SELECT firstName FROM ProjectManagerSchema.Users WHERE UserId = " + userId;
        string lastNameSql = "SELECT lastName FROM ProjectManagerSchema.Users WHERE UserId = " + userId;

        int userIdFromDb = _dapper.LoadSingleData<int>(userIdSql);
        string firstName = _dapper.LoadSingleData<string>(firstNameSql);
        string lastName = _dapper.LoadSingleData<string>(lastNameSql);

        return Ok(new Dictionary<string, string>() {
            {"token", _authHelper.CreateToken(userIdFromDb, firstName, lastName)}
        });
    }

}