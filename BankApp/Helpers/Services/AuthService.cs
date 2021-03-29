using System.Threading.Tasks;
using BankApp.Dtos.Auth;
using BankApp.Exceptions;
using BankApp.Interfaces.Builders;
using BankApp.Interfaces.Services;
using BankApp.Models;
using Microsoft.AspNetCore.Identity;

namespace BankApp.Helpers.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtBuilder _jwtBuilder;

        public AuthService(UserManager<ApplicationUser> userManager, IJwtBuilder jwtBuilder)
        {
            _userManager = userManager;
            _jwtBuilder = jwtBuilder;
        }

        public async Task<AuthResultDto> LoginAsync(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                throw new InvalidLoginException("Invalid login attempt.");

            var roles = await _userManager.GetRolesAsync(user);
            var claimsIdentity = _jwtBuilder.GenerateClaimsIdentity(user, roles);
            var jwt = _jwtBuilder.GenerateEncodedToken(claimsIdentity);

            return new AuthResultDto
            {
                Token = jwt
            };
        }

        public async Task<AuthResultDto> RefreshTokenAsync(string token, string refreshToken)
        {
            throw new System.NotImplementedException();
        }
    }
}