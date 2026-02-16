using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventHub.Core.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace EventHub.Infrastructure.Helpers
{
    public class JwtTokenGenerator
    {
        private readonly IConfiguration _config;

        public JwtTokenGenerator(IConfiguration config)
        {
            _config = config;
        }
        public string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim("id", user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expireMinutesString = _config["Jwt:ExpireMinutes"];
            double expireMinutes;
            if (!double.TryParse(expireMinutesString, out expireMinutes))
            {
                expireMinutes = 60; 
            }

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }

        public DateTime GetRefreshTokenExpiry()
        {
            var refreshTokenDaysString = _config["Jwt:RefreshTokenExpireDays"];
            int refreshTokenDays;
            if (!int.TryParse(refreshTokenDaysString, out refreshTokenDays))
            {
                refreshTokenDays = 7; // Default 7 days
            }
            return DateTime.UtcNow.AddDays(refreshTokenDays);
        }

    }

}
