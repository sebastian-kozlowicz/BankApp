using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using BankApp.Configuration;
using BankApp.Data;
using BankApp.Dtos.Auth;
using BankApp.Exceptions;
using BankApp.Interfaces.Builders;
using BankApp.Interfaces.Services;
using BankApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BankApp.Helpers.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtBuilder _jwtBuilder;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthService(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            IJwtBuilder jwtBuilder)
        {
            _context = context;
            _userManager = userManager;
            _jwtBuilder = jwtBuilder;
        }

        public async Task<AuthResultDto> LoginAsync(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                throw new InvalidLoginException("Invalid login attempt");

            var roles = await _userManager.GetRolesAsync(user);
            var claimsIdentity = _jwtBuilder.GenerateClaimsIdentity(user, roles);
            var securityToken = _jwtBuilder.GenerateSecurityToken(claimsIdentity);

            var newRefreshToken = new RefreshTokenData
            {
                RefreshToken = Guid.NewGuid().ToString(),
                Jti = securityToken.Id,
                CreationDate = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddDays(1),
                IsUsed = false,
                IsInvalidated = false,
                ApplicationUserId = user.Id
            };

            await _context.RefreshTokens.AddAsync(newRefreshToken);
            await _context.SaveChangesAsync();

            return new AuthResultDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                RefreshToken = newRefreshToken.RefreshToken
            };
        }

        public async Task<AuthResultDto> RefreshTokenAsync(string token, string refreshToken)
        {
            var validatedToken = _jwtBuilder.GetPrincipalFromToken(token);

            if (validatedToken == null)
                throw new RefreshTokenException("Invalid token");

            var expirationDateUnixEpoch =
                long.Parse(validatedToken.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Exp).Value);
            var expirationDateUtc = EpochTime.DateTime(expirationDateUnixEpoch);

            if (expirationDateUtc > DateTime.UtcNow)
                throw new RefreshTokenException("JWT is not expired yet");

            var jti = validatedToken.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Jti).Value;
            var refreshTokenInDb = await _context.RefreshTokens.SingleOrDefaultAsync(t => t.RefreshToken == refreshToken);

            if (refreshTokenInDb == null)
                throw new RefreshTokenException("Refresh token in database does not exist");

            refreshTokenInDb.IsUsed = true;
            await _context.SaveChangesAsync();

            var userId = int.Parse(validatedToken.Claims.Single(c => c.Type == CustomClaimTypes.UserId).Value);
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == userId);

            var roles = await _userManager.GetRolesAsync(user);
            var claimsIdentity = _jwtBuilder.GenerateClaimsIdentity(user, roles);
            var securityToken = _jwtBuilder.GenerateSecurityToken(claimsIdentity);

            var newRefreshToken = new RefreshTokenData
            {
                RefreshToken = Guid.NewGuid().ToString(),
                Jti = securityToken.Id,
                CreationDate = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddDays(1),
                IsUsed = false,
                IsInvalidated = false,
                ApplicationUserId = userId
            };

            await _context.RefreshTokens.AddAsync(newRefreshToken);
            await _context.SaveChangesAsync();

            return new AuthResultDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                RefreshToken = newRefreshToken.RefreshToken
            };
        }
    }
}