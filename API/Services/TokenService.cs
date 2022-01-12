using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    // Class for the jwt structure 
    public class TokenService
    {
        // The read fields and constructor for the token services
        // Accepts any config parameters
        private readonly IConfiguration _config;
        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        // Method for construction the jwt token
        public string CreateToken(AppUser user)
        {
            // A list of claims to identify the user with the token
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
            };
            // A key assigned and then signed with a sha512 Signature for security
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["TokenKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            // The token put data being put together. Given the list of claims, an expiration date and signed key
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };
            // The jwt
            var tokenHandler = new JwtSecurityTokenHandler();
            // Assigning the token values to the jwt
            var token = tokenHandler.CreateToken(tokenDescriptor);
            // Sending up the newly constructed jwt token
            return tokenHandler.WriteToken(token);
        }
    }
}