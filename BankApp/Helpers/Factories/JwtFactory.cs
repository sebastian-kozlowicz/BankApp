using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using BankApp.Configuration;
using BankApp.Interfaces;
using BankApp.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BankApp.Helpers.Factories
{
    public class JwtFactory : IJwtFactory
    {
        private readonly JwtIssuerOptions _jwtOptions;

        public JwtFactory(IOptions<JwtIssuerOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
            ThrowIfInvalidOptions(_jwtOptions);
        }

        public string GenerateEncodedToken(ClaimsIdentity claimsIdentity)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                NotBefore = _jwtOptions.NotBefore,
                Expires = _jwtOptions.Expiration,
                SigningCredentials = _jwtOptions.SigningCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);
            return token;
        }

        public ClaimsIdentity GenerateClaimsIdentity(ApplicationUser user, IEnumerable<string> roles)
        {
            var claimsIdentity = new ClaimsIdentity(new GenericIdentity(user.Email, "Token"), new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, _jwtOptions.Jti),
                new Claim(JwtRegisteredClaimNames.Iat, EpochTime.GetIntDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
                new Claim(CustomClaimTypes.UserId, user.Id.ToString(), ClaimValueTypes.Integer32)
            });

            claimsIdentity.AddClaims(roles.Select(r => new Claim(ClaimTypes.Role, r)));
            return claimsIdentity;
        }

        private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));

            if (options.SigningCredentials == null)
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));

            if (options.Jti == null)
                throw new ArgumentNullException(nameof(JwtIssuerOptions.Jti));
        }
    }
}
