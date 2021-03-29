using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using BankApp.Configuration;
using BankApp.Interfaces.Builders;
using BankApp.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BankApp.Helpers.Builders
{
    public class JwtBuilder : IJwtBuilder
    {
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly TokenValidationParameters _tokenValidationParameters;

        public JwtBuilder(IOptions<JwtIssuerOptions> jwtOptions, TokenValidationParameters tokenValidationParameters)
        {
            _jwtOptions = jwtOptions.Value;
            _tokenValidationParameters = tokenValidationParameters;
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

        public ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);

                return !IsJwtWithValidSecurityAlgorithm(validatedToken) ? null : principal;
            }
            catch
            {
                return null;
            }
        }

        private static bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return validatedToken is JwtSecurityToken jwtSecurityToken &&
                   jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                       StringComparison.InvariantCultureIgnoreCase);
        }
    }
}