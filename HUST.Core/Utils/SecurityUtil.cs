using HUST.Core.Constants;
using HUST.Core.Models.DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;

namespace HUST.Core.Utils
{
    public class SecurityUtil
    {
        /// <summary>
        /// Hàm băm mật khẩu
        /// </summary>
        /// <param name="rawPassword"></param>
        /// <returns></returns>
        public static string HashPassword(string rawPassword)
        {
            return BC.HashPassword(rawPassword);
        }

        /// <summary>
        /// Hàm xác minh mật khẩu
        /// </summary>
        /// <param name="rawPassword"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static bool VerifyPassword(string rawPassword, string hash)
        {
            return BC.Verify(rawPassword, hash);
        }

        /// <summary>
        /// Gen jwt token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static string GenerateToken(User user, IConfiguration configuration)
        {
            var appSettings = configuration.GetSection(AppSettingKey.AppSettingsSection);
            var secretKey = appSettings[AppSettingKey.JwtSecretKey];
            var issuer = appSettings[AppSettingKey.JwtIssuer];
            var audience = appSettings[AppSettingKey.JwtAudience];

            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                        new Claim("UserId", Guid.NewGuid().ToString()),
                        new Claim("UserName", user.UserName),
                        new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Email, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                     }),
                Expires = DateTime.UtcNow.AddMinutes(1),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials
                (new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = new JwtSecurityTokenHandler().CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);
            return jwtToken;
        }
    }
}
