using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;

namespace ProjectManagerAPI.Helpers;

public class AuthHelper
{
    // Configuration object that contains the Connection String for the Database
    private readonly IConfiguration _config;

    // Constructor for DataContextDapper object
    public AuthHelper(IConfiguration config)
    {
        _config = config;
    }

    // Function used to encrypt the passwords
    public byte[] GetPasswordHash(string password, byte[] passwordSalt)
    {
        // Pasword Salt 
        string passwordSaltPlusString = _config.GetSection("AppSettings:PasswordKey").Value
            + Convert.ToBase64String(passwordSalt);

        // Password Encryption
        byte[] passwordHash = KeyDerivation.Pbkdf2(
            password: password,
            salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 256 / 8
        );

        return passwordHash;
    }

    // Function used to create a JWT
    public string CreateToken(int userId, string firstName, string lastName)
    {
        // Claims that are going to be present inside the Token
        Claim[] claims = new Claim[] {
            new("userId", userId.ToString()),
            new("firstName", firstName),
            new("lastName", lastName)
        };

        // Getting the credentials for the Token
        string? tokenKeyString = _config.GetSection("AppSettings:TokenKey").Value;
        SymmetricSecurityKey tokenKey = new(Encoding.UTF8.GetBytes(tokenKeyString ?? ""));

        SigningCredentials credentials = new(tokenKey, SecurityAlgorithms.HmacSha512Signature);

        // Token descriptor
        SecurityTokenDescriptor descriptor = new()
        {
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = credentials,
            Expires = DateTime.Now.AddDays(1)
        };

        // Creation of the Token
        JwtSecurityTokenHandler tokenHandler = new();
        SecurityToken token = tokenHandler.CreateToken(descriptor);

        return tokenHandler.WriteToken(token);
    }
}