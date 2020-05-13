using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Jiggswap.Application.Users.Dtos;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Jiggswap.Api.Services
{
    public interface ITokenBuilder
    {
        string CreateToken(AuthorizedUserResponse user);
    }

    public class TokenBuilder : ITokenBuilder
    {
        private readonly string _jwtKey;

        public TokenBuilder(IConfiguration config)
        {
            _jwtKey = config["Jwt:Key"];
        }

        public string CreateToken(AuthorizedUserResponse user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtKey)), SecurityAlgorithms.HmacSha256Signature)
            };

            return tokenHandler.WriteToken(tokenHandler.CreateToken(descriptor));
        }
    }
}