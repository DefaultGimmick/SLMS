using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SLMS.Tools
{
    public class JwtUtils
    {
        private readonly IOptions<Audience> m_Settings;

        public JwtUtils(IOptions<Audience> settings)
        {
            m_Settings = settings;
        }

        /// <summary>
        /// 生成token
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public string CreateToken(string username)
        {
            var now = DateTime.UtcNow;
            var expires = now.Add(m_Settings.Value.TokenExpiration);

            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, now.ToUniversalTime().ToString(), ClaimValueTypes.Integer64)
            };

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(m_Settings.Value.Secret));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var tokenOptions = new JwtSecurityToken(
                issuer: m_Settings.Value.Iss,
                audience: m_Settings.Value.Aud,
                claims: claims,
                notBefore: now,
                expires: expires,
                signingCredentials: signingCredentials
            );

            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return token;
        }
    }
}
