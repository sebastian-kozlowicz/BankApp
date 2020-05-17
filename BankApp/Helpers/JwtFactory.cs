using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using BankApp.Configuration;
using BankApp.Enumerators;
using BankApp.Interfaces;
using BankApp.Models;
using Microsoft.Extensions.Options;

namespace BankApp.Helpers
{
    public class JwtFactory : IJwtFactory
    {
        private readonly JwtIssuerOptions _jwtOptions;

        public JwtFactory(IOptions<JwtIssuerOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
            ThrowIfInvalidOptions(_jwtOptions);
        }

        public async Task<string> GenerateEncodedToken(string email, ClaimsIdentity claimsIdentity)
        {
            var claims = new[]
            {
                 new Claim(JwtRegisteredClaimNames.Sub, email),
                 new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                 new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
                 claimsIdentity.FindFirst("userId"),
                 claimsIdentity.FindFirst("administrator"),
                 claimsIdentity.FindFirst("customer"),
                 claimsIdentity.FindFirst("employee"),
                 claimsIdentity.FindFirst("manager"),
             };

            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: _jwtOptions.NotBefore,
                expires: _jwtOptions.Expiration,
                signingCredentials: _jwtOptions.SigningCredentials);

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        public ClaimsIdentity GenerateClaimsIdentity(ApplicationUser user, IList<string> roles)
        {
            return new ClaimsIdentity(new GenericIdentity(user.Email, "Token"), new[]
            {
                new Claim("userId", user.Id),
                new Claim("administrator", RoleHelper.IsUserInRole(roles, UserRoles.Administrator).ToString(), ClaimValueTypes.Boolean),
                new Claim("customer", RoleHelper.IsUserInRole(roles, UserRoles.Customer).ToString(), ClaimValueTypes.Boolean),
                new Claim("employee", RoleHelper.IsUserInRole(roles, UserRoles.Employee).ToString(), ClaimValueTypes.Boolean),
                new Claim("manager", RoleHelper.IsUserInRole(roles, UserRoles.Manager).ToString(), ClaimValueTypes.Boolean),
            });
        }

        private static long ToUnixEpochDate(DateTime date)
          => (long)Math.Round((date.ToUniversalTime() -
                               new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                              .TotalSeconds);

        private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null) 
                throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));

            if (options.SigningCredentials == null)
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));

            if (options.JtiGenerator == null)
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
        }
    }
}
